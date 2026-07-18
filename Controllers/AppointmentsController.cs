using Clinic.DTOS;
using Clinic.Models;
using Clinic.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Clinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentRepository _repo;
        private readonly AppointmentTagRepository _tagRepo;
        private readonly CancellationCommandRepository _cancelRepo;

        public AppointmentsController(
            AppointmentRepository repo,
            AppointmentTagRepository tagRepo,
            CancellationCommandRepository cancelRepo)
        {
            _repo = repo;
            _tagRepo = tagRepo;
            _cancelRepo = cancelRepo;
        }

        private static AppointmentDetailDto ToDetailDto(Appointment a) => new AppointmentDetailDto
        {
            AppointmentId = a.AppointmentId,
            PatientId = a.PatientId,
            DoctorId = a.DoctorId,
            AppointmentDate = a.AppointmentDate,
            AppointmentTime = a.AppointmentTime,
            Status = a.Status.ToString(),
            Message = a.Message,
            CreatedAt = a.CreatedAt,
            Patient = a.Patient == null ? null : new PatientInfoDto
            {
                Id = a.Patient.Id,
                FullName = $"{a.Patient.FirstName} {a.Patient.LastName}".Trim(),
                Email = a.Patient.Email,
                Phone = a.Patient.PhoneNumber
            },
            Doctor = a.Doctor == null ? null : new DoctorInfoDto
            {
                Id = a.Doctor.Id,
                Name = a.Doctor.Name,
                SpecialtyName = a.Doctor.Specialty?.Name,
                Phone = a.Doctor.Phone,
                Email = a.Doctor.Email,
                ConsultationFees = a.Doctor.ConsultationFees
            },
            Tags = a.Tags?.Select(t => new TagResponseDto
            {
                Id = t.Id,
                TagName = t.TagName,
                CreatedAt = t.CreatedAt
            }).ToList() ?? new List<TagResponseDto>()
        };

        // GET: api/appointments
        // GET: api/appointments?mine=true       -> only the logged-in patient's own appointments
        // GET: api/appointments?doctorId=3       -> only appointments for a given doctor (doctor dashboard)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool mine = false, [FromQuery] int? doctorId = null)
        {
            var appointments = await _repo.GetAllAsync();

            if (mine)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                appointments = appointments.Where(a => a.PatientId == userId).ToList();
            }

            if (doctorId.HasValue)
            {
                appointments = appointments.Where(a => a.DoctorId == doctorId.Value).ToList();
            }

            return Ok(appointments.Select(ToDetailDto));
        }

        // GET: api/appointments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _repo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound("الموعد مش موجود");
            return Ok(ToDetailDto(appointment));
        }

        // POST: api/appointments
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var appointment = new Appointment
            {
                PatientId = userId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = dto.AppointmentTime,
                Message = dto.Message
            };

            var created = await _repo.CreateAsync(appointment);
            var full = await _repo.GetByIdAsync(created.AppointmentId);
            return CreatedAtAction(nameof(GetById), new { id = created.AppointmentId }, ToDetailDto(full!));
        }

        // PUT: api/appointments/5/confirm
        [Authorize]
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _repo.ConfirmAsync(id);
            if (!result) return NotFound("الموعد مش موجود");
            return Ok("تم تأكيد الموعد");
        }

        // PUT: api/appointments/5/cancel
        [Authorize]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            // Command Pattern: Pass the current user ID so cancellation is recorded
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _repo.CancelAsync(id, userId);
            if (!result) return NotFound("الموعد مش موجود");
            return Ok("تم إلغاء الموعد");
        }

        // PUT: api/appointments/5/complete
        [Authorize]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var result = await _repo.CompleteAsync(id);
            if (!result) return NotFound("الموعد مش موجود");
            return Ok("تم إنهاء الموعد");
        }

        // DELETE: api/appointments/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repo.DeleteAsync(id);
            if (!result) return NotFound("الموعد مش موجود");
            return NoContent();
        }

        // =====================================================
        // Dev 6: Decorator Pattern — Dynamic Tags
        // =====================================================

        /// <summary>
        /// POST: api/appointments/{id}/tags
        /// Decorator Pattern: Add a dynamic tag (VIP, Urgent, LateFee) to an appointment.
        /// </summary>
        [HttpPost("{id}/tags")]
        public async Task<IActionResult> AddTag(int id, [FromBody] AddTagDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TagName))
                return BadRequest(new { error = "Tag name is required." });

            try
            {
                var tag = await _tagRepo.AddTagAsync(id, dto.TagName.Trim());
                return Ok(new TagResponseDto
                {
                    Id = tag.Id,
                    TagName = tag.TagName,
                    CreatedAt = tag.CreatedAt
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/appointments/{id}/tags
        /// Decorator Pattern: Get all tags for an appointment.
        /// </summary>
        [HttpGet("{id}/tags")]
        public async Task<IActionResult> GetTags(int id)
        {
            var tags = await _tagRepo.GetTagsByAppointmentIdAsync(id);
            var response = tags.Select(t => new TagResponseDto
            {
                Id = t.Id,
                TagName = t.TagName,
                CreatedAt = t.CreatedAt
            });
            return Ok(response);
        }

        /// <summary>
        /// DELETE: api/appointments/{id}/tags/{tagName}
        /// Decorator Pattern: Remove a specific tag from an appointment.
        /// </summary>
        [HttpDelete("{id}/tags/{tagName}")]
        public async Task<IActionResult> RemoveTag(int id, string tagName)
        {
            var result = await _tagRepo.RemoveTagAsync(id, tagName);
            if (!result)
                return NotFound(new { error = $"Tag '{tagName}' not found on appointment {id}." });
            return Ok(new { message = $"Tag '{tagName}' removed successfully." });
        }

        // =====================================================
        // Dev 6: Command Pattern — Undo Cancellation
        // =====================================================

        /// <summary>
        /// POST: api/appointments/undo-last-cancel
        /// Command Pattern: Reverses the last canceled appointment for the current user.
        /// Demonstrates the Command Pattern's capability to store state history.
        /// </summary>
        [Authorize]
        [HttpPost("undo-last-cancel")]
        public async Task<IActionResult> UndoLastCancel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var result = await _cancelRepo.UndoLastCancellationAsync(userId);
                if (result == null)
                    return NotFound(new { error = "No cancellations found to undo." });

                var (appointment, previousState) = result.Value;

                return Ok(new UndoCancelResponseDto
                {
                    Message = "Cancellation reversed successfully.",
                    RestoredAppointmentId = appointment.AppointmentId,
                    PreviousState = previousState
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}