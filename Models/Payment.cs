namespace Clinic.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int AppointmentID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionID { get; set; }

        public Appointment Appointment { get; set; }


    }
}
