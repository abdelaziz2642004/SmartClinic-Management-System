using Clinic.Data;
using Clinic.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Repositories
{
    /// <summary>
    /// Decorator Pattern Repository: Manages dynamic tags on appointments.
    /// </summary>
    public class AppointmentTagRepository
    {
        private readonly AppDbContext _context;

        public AppointmentTagRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add a tag to an appointment (Decorator Pattern).
        /// Validates that the appointment exists and the tag isn't duplicated.
        /// </summary>
        public async Task<AppointmentTag> AddTagAsync(int appointmentId, string tagName)
        {
            // Verify appointment exists
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {appointmentId} not found.");

            // Check for duplicate tag
            var existingTag = await _context.AppointmentTags
                .FirstOrDefaultAsync(t => t.AppointmentId == appointmentId && t.TagName == tagName);
            if (existingTag != null)
                throw new InvalidOperationException($"Tag '{tagName}' already exists on this appointment.");

            var tag = new AppointmentTag
            {
                AppointmentId = appointmentId,
                TagName = tagName,
                CreatedAt = DateTime.UtcNow
            };

            _context.AppointmentTags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        /// <summary>
        /// Get all tags for an appointment.
        /// </summary>
        public async Task<List<AppointmentTag>> GetTagsByAppointmentIdAsync(int appointmentId)
        {
            return await _context.AppointmentTags
                .Where(t => t.AppointmentId == appointmentId)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Remove a specific tag from an appointment.
        /// </summary>
        public async Task<bool> RemoveTagAsync(int appointmentId, string tagName)
        {
            var tag = await _context.AppointmentTags
                .FirstOrDefaultAsync(t => t.AppointmentId == appointmentId && t.TagName == tagName);
            if (tag == null) return false;

            _context.AppointmentTags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
