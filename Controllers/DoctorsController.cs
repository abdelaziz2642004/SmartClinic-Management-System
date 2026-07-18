using Clinic.Data;
using Clinic.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
        }

        private static IQueryable<DoctorDto> ProjectToDto(IQueryable<Models.Doctor> query)
        {
            return query.Select(d => new DoctorDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
                ConsultationFees = d.ConsultationFees,
                Description = d.Description,
                SpecialtyID = d.SpecialtyID,
                SpecialtyName = d.Specialty != null ? d.Specialty.Name : null,
                ImagePath = d.ImagePath
            });
        }

        // GET: api/doctors
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await ProjectToDto(_context.Doctors.Include(d => d.Specialty)).ToListAsync();
            return Ok(doctors);
        }

        // GET: api/doctors/me
        // Returns the Doctor profile linked to the currently logged-in account (if any).
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var entity = await _context.Doctors.Include(d => d.Specialty).FirstOrDefaultAsync(d => d.UserId == userId);
            if (entity == null)
                return NotFound(new { error = "No doctor profile is linked to this account yet." });

            return Ok(new DoctorDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Phone = entity.Phone,
                ConsultationFees = entity.ConsultationFees,
                Description = entity.Description,
                SpecialtyID = entity.SpecialtyID,
                SpecialtyName = entity.Specialty?.Name,
                ImagePath = entity.ImagePath
            });
        }

        // GET: api/doctors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await ProjectToDto(_context.Doctors.Include(d => d.Specialty))
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound(new { error = "Doctor not found." });

            return Ok(doctor);
        }

        // GET: api/doctors/specialty/Cardiology
        [HttpGet("specialty/{specialtyName}")]
        public async Task<IActionResult> GetBySpecialty(string specialtyName)
        {
            var doctors = await ProjectToDto(
                    _context.Doctors
                        .Include(d => d.Specialty)
                        .Where(d => d.Specialty.Name.ToLower() == specialtyName.ToLower()))
                .ToListAsync();

            return Ok(doctors);
        }

        // PUT: api/doctors/5
        // NOTE: Doctor accounts are not yet linked to an AspNetUsers identity (see project notes),
        // so this only requires *some* logged-in user rather than verifying it's this exact doctor.
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDoctorDto dto)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound(new { error = "Doctor not found." });

            if (!string.IsNullOrWhiteSpace(dto.Name)) doctor.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) doctor.Phone = dto.Phone;
            if (dto.Description != null) doctor.Description = dto.Description;
            if (dto.ConsultationFees > 0) doctor.ConsultationFees = dto.ConsultationFees;

            await _context.SaveChangesAsync();

            var updated = await ProjectToDto(_context.Doctors.Include(d => d.Specialty))
                .FirstOrDefaultAsync(d => d.Id == id);
            return Ok(updated);
        }
        // POST: api/doctors/5/photo  (multipart/form-data, field name "file")
        [Authorize]
        [HttpPost("{id}/photo")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound(new { error = "Doctor not found." });

            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file was uploaded." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { error = "Only .jpg, .jpeg, .png, or .webp images are allowed." });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "doctors");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{id}-{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(doctor.ImagePath))
            {
                var oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doctor.ImagePath);
                if (System.IO.File.Exists(oldFullPath))
                {
                    try { System.IO.File.Delete(oldFullPath); } catch { /* non-fatal */ }
                }
            }

            doctor.ImagePath = $"images/doctors/{fileName}";
            await _context.SaveChangesAsync();

            return Ok(new { imagePath = doctor.ImagePath });
        }
    }
}
