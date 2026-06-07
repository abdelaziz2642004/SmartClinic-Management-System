using Clinic.Models;
using Microsoft.AspNetCore.Identity;

public class MedicalRecordProxy
{
    private readonly UserManager<User> _userManager;

    public MedicalRecordProxy(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> CanAccessRecord(string userId, string targetPatientId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user);

       
        if (roles.Contains("Admin") || roles.Contains("Doctor"))
            return true;

        
        return userId == targetPatientId;
    }
}