using Microsoft.AspNetCore.Identity;

namespace Clinic.Services
{
    public class RoleFactory
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleFactory(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task CreateRolesAsync()
        {
            string[] roleNames = { "Admin", "Doctor", "Patient" };
            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
