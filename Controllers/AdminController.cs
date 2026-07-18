using Clinic.Data;
using Clinic.DTOS;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var doctorLinks = await _context.Doctors
    .Where(d => d.UserId != null)
    .ToDictionaryAsync(d => d.UserId!, d => new { d.Id, d.ImagePath });

            var result = new List<UserAdminDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var link = doctorLinks.TryGetValue(user.Id, out var linkVal) ? linkVal : null;
                result.Add(new UserAdminDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList(),
                    DoctorId = link?.Id,
                    DoctorImagePath = link?.ImagePath
                });
            }
            return Ok(result.OrderByDescending(u => u.CreatedAt));
        }

        // PUT: api/admin/users/{id}/role
        // Sets a simple role (Patient or Admin). To grant Doctor access use /promote-doctor instead,
        // since that also needs to create the linked Doctor profile (specialty, fee, etc).
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> ChangeRole(string id, ChangeRoleDto dto)
        {
            if (dto.Role != "Patient" && dto.Role != "Admin")
                return BadRequest(new { error = "Role must be 'Patient' or 'Admin'. Use /promote-doctor to grant Doctor access." });

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { error = "User not found." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);

            return Ok(new { message = $"Role updated to {dto.Role}." });
        }

        // POST: api/admin/users/{id}/promote-doctor
        [HttpPost("users/{id}/promote-doctor")]
        public async Task<IActionResult> PromoteToDoctor(string id, PromoteDoctorDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { error = "User not found." });

            var specialty = await _context.Specialties.FindAsync(dto.SpecialtyID);
            if (specialty == null) return BadRequest(new { error = "Specialty not found." });

            var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == id);
            if (existingDoctor != null)
                return BadRequest(new { error = "This user already has a doctor profile." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, "Doctor");

            var doctor = new Doctor
            {
                Name = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                Password = "N/A", // this Doctor table predates Identity and isn't used for login
                Phone = user.PhoneNumber ?? "",
                ConsultationFees = dto.ConsultationFees,
                Description = dto.Description ?? "",
                SpecialtyID = dto.SpecialtyID,
                UserId = user.Id
            };
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User promoted to Doctor.", doctorId = doctor.Id });
        }
    }
}
