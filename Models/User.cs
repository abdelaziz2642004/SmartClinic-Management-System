using Microsoft .AspNetCore.Identity;
namespace Clinic.Models
{
    public class User : IdentityUser
    {
   
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
