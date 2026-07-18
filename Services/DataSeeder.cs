using Clinic.Data;
using Clinic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Services
{
    /// <summary>
    /// Seeds Specialties + Doctors + one Admin account so the app has real data to show
    /// (and someone who can actually access the admin panel) the first time it runs.
    /// The three doctors mirror the three demo cards that used to be hardcoded directly
    /// inside Doctors.html, so the site looks the same but now the data comes from the DB.
    /// </summary>
    public class DataSeeder
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        // CHANGE THIS PASSWORD after your first login in production.
        public const string SeedAdminEmail = "admin@mediclinic.com";
        public const string SeedAdminPassword = "Admin@123";

        public DataSeeder(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            if (!await _context.Specialties.AnyAsync())
            {
                _context.Specialties.AddRange(
                    new Specialty { Name = "Cardiology" },
                    new Specialty { Name = "Orthopedic" },
                    new Specialty { Name = "Dentist" }
                );
                await _context.SaveChangesAsync();
            }

            // Demo doctors are no longer auto-seeded. Add real doctors via the Admin panel
            // (Register an account, then use Promote to Doctor) so Home/Doctors pages only
            // ever show doctors that were actually added.

            // Seed one Admin account, since nothing can self-register as Admin.
            var existingAdmin = await _userManager.FindByEmailAsync(SeedAdminEmail);
            if (existingAdmin == null)
            {
                var admin = new User
                {
                    UserName = SeedAdminEmail,
                    Email = SeedAdminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    Gender = "N/A",
                    Address = "N/A",
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(admin, SeedAdminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}