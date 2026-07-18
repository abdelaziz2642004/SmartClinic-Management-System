using Clinic.Data;
using Clinic.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialtiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SpecialtiesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/specialties
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var specialties = await _context.Specialties
                .Select(s => new SpecialtyDto
                {
                    SpecialtyID = s.SpecialtyID,
                    Name = s.Name,
                    DoctorsCount = s.Doctors.Count
                })
                .ToListAsync();

            return Ok(specialties);
        }
    }
}
