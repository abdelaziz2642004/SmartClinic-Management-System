using Clinic.Data;
using Clinic.Models;
using Clinic.States;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Repositories
{
    /// <summary>
    /// Command Pattern Repository: Manages cancellation history and undo operations.
    /// </summary>
    public class CancellationCommandRepository
    {
        private readonly AppDbContext _context;

        public CancellationCommandRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Record a cancellation command before the appointment is actually cancelled.
        /// This stores the history needed for undo.
        /// </summary>
        public async Task<CancellationCommand> RecordCancellationAsync(int appointmentId, string previousState, string? cancelledByUserId)
        {
            var command = new CancellationCommand
            {
                AppointmentId = appointmentId,
                PreviousState = previousState,
                CancelledAt = DateTime.UtcNow,
                CancelledByUserId = cancelledByUserId,
                IsUndone = false
            };

            _context.CancellationCommands.Add(command);
            await _context.SaveChangesAsync();
            return command;
        }

        /// <summary>
        /// Undo the last cancellation for a specific user.
        /// Pops the most recent non-undone cancellation and restores the appointment's state.
        /// </summary>
        public async Task<(Appointment appointment, string previousState)?> UndoLastCancellationAsync(string? userId)
        {
            // Find the most recent non-undone cancellation (optionally filtered by user)
            var query = _context.CancellationCommands
                .Where(c => !c.IsUndone)
                .OrderByDescending(c => c.CancelledAt)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(c => c.CancelledByUserId == userId);
            }

            var lastCommand = await query.FirstOrDefaultAsync();
            if (lastCommand == null)
                return null;

            // Find the appointment
            var appointment = await _context.Appointments.FindAsync(lastCommand.AppointmentId);
            if (appointment == null)
                return null;

            // Use the State Pattern to validate and execute the undo
            var stateContext = new AppointmentContext(appointment);
            stateContext.UndoCancellation(); // This will throw if the appointment is not in Cancelled state

            // Mark the command as undone
            lastCommand.IsUndone = true;

            await _context.SaveChangesAsync();

            return (appointment, lastCommand.PreviousState);
        }

        /// <summary>
        /// Get all cancellation history for an appointment.
        /// </summary>
        public async Task<List<CancellationCommand>> GetCancellationHistoryAsync(int appointmentId)
        {
            return await _context.CancellationCommands
                .Where(c => c.AppointmentId == appointmentId)
                .OrderByDescending(c => c.CancelledAt)
                .ToListAsync();
        }
    }
}
