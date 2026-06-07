namespace Clinic.DTOS
{
    public class RegisterDto
    {
       public string firstName { get; set; }
        public string lastName { get; set; }
        public string FullName { get; set; }
        public string phone { get; set; }
        public DateTime  ? Birthday { get; set; }
        public string ? gender { get; set; }
        public string ? address { get; set; }
        public string Email { get; set; }
            public string Password { get; set; }

        public string Role { get; set; } = "Patient"; 
    }

}
