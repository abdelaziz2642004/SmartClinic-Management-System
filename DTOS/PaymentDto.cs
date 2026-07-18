namespace Clinic.DTOS
{
    public class CreatePaymentDto
    {
        public int AppointmentId { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
    }

    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int AppointmentID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionID { get; set; }
    }
}
