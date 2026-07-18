using Clinic.Data;
using Clinic.DTOS;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        private static PaymentResponseDto ToDto(Payment p) => new PaymentResponseDto
        {
            Id = p.Id,
            AppointmentID = p.AppointmentID,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            PaymentStatus = p.PaymentStatus,
            PaymentDate = p.PaymentDate,
            TransactionID = p.TransactionID
        };

        // POST: api/payments
        // Records payment for an appointment that belongs to the current user.
        // There is no real payment gateway here (student project), so the payment
        // is simulated as immediately "Completed" once billing details are submitted.
        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == dto.AppointmentId);

            if (appointment == null)
                return NotFound(new { error = "الموعد مش موجود" });

            if (appointment.PatientId != userId)
                return Forbid();

            var existing = await _context.Payments.FirstOrDefaultAsync(p => p.AppointmentID == dto.AppointmentId);
            if (existing != null)
                return Ok(ToDto(existing));

            var payment = new Payment
            {
                AppointmentID = appointment.AppointmentId,
                Amount = appointment.Doctor?.ConsultationFees ?? 0,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Completed",
                PaymentDate = DateTime.UtcNow,
                TransactionID = Guid.NewGuid().ToString("N")
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(ToDto(payment));
        }

        // GET: api/payments/appointment/5
        [HttpGet("appointment/{appointmentId}")]
        public async Task<IActionResult> GetByAppointment(int appointmentId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.AppointmentID == appointmentId);
            if (payment == null)
                return NotFound(new { error = "No payment found for this appointment." });

            return Ok(ToDto(payment));
        }
    }
}
