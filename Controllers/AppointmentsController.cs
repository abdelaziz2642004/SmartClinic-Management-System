using Clinic.DTOS;
using Clinic.Models;
using Clinic.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentRepository _repo;

        public AppointmentsController(AppointmentRepository repo)
        {
            _repo = repo;
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _repo.GetAllAsync();
            return Ok(appointments);
        }

        // GET: api/appointments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _repo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound("الموعد مش موجود");
            return Ok(appointment);
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentDto dto)
        {
            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = dto.AppointmentTime,
                Message = dto.Message
            };

            var created = await _repo.CreateAsync(appointment);
            return CreatedAtAction(nameof(GetById), new { id = created.AppointmentId }, created);
        }

        // PUT: api/appointments/5/confirm
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _repo.ConfirmAsync(id);
            if (!result) return NotFound("الموعد مش موجود");
            return Ok("تم تأكيد الموعد");
        }

        // PUT: api/appointments/5/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _repo.CancelAsync(id);
            if (!result) return NotFound("الموعد مش موجود");
            return Ok("تم إلغاء الموعد");
        }

        // PUT: api/appointments/5/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var result = await _repo.CompleteAsync(id);
            if (!result) return NotFound("الموعد مش موجود");
            return Ok("تم إنهاء الموعد");
        }

        // DELETE: api/appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repo.DeleteAsync(id);
            if (!result) return NotFound("الموعد مش موجود");
            return NoContent();
        }
    }
}