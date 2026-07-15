using Clinic.Data;
using Clinic.Models;
using Clinic.States;
using Clinic.Observers;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Repositories
{
    public class AppointmentRepository
    {
        private readonly AppDbContext _context;
        private readonly IAppointmentSubject _notifier;
        private readonly Clinic.Calendar.ICalendarAdapter _calendarAdapter;

        public AppointmentRepository(AppDbContext context, IAppointmentSubject notifier, Clinic.Calendar.ICalendarAdapter calendarAdapter)
        {
            _context = context;
            _notifier = notifier;
            _calendarAdapter = calendarAdapter;
        }
        public async Task<List<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Tags) 
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Tags) 
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.Status = AppointmentStatus.Pending;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();


            try
            {
                await _calendarAdapter.CreateEventAsync(appointment);
            }
            catch (Exception)
            {
            }

            return appointment;
        }

        public async Task<bool> ConfirmAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;
            var context = new AppointmentContext(appointment);
            context.Confirm();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAsync(int id, string? cancelledByUserId = null)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            var previousState = appointment.Status.ToString();
            var command = new CancellationCommand
            {
                AppointmentId = appointment.AppointmentId,
                PreviousState = previousState,
                CancelledAt = DateTime.UtcNow,
                CancelledByUserId = cancelledByUserId,
                IsUndone = false
            };
            _context.CancellationCommands.Add(command);

            var context = new AppointmentContext(appointment);
            context.Cancel();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;
            var context = new AppointmentContext(appointment);
            context.Complete();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
