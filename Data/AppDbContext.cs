using Clinic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public new DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Report> Reports { get; set; }

        // Dev 6: Decorator Pattern (Tags) + Command Pattern (Undo Cancellation)
        public DbSet<AppointmentTag> AppointmentTags { get; set; }
        public DbSet<CancellationCommand> CancellationCommands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.AppointmentTime })
                .IsUnique();

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.PatientId, a.AppointmentDate, a.AppointmentTime })
                .IsUnique();

            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Doctor>()
                .Property(d => d.ConsultationFees)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            // Dev 6: AppointmentTag (Decorator Pattern)
            modelBuilder.Entity<AppointmentTag>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasOne(t => t.Appointment)
                    .WithMany(a => a.Tags)
                    .HasForeignKey(t => t.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => new { t.AppointmentId, t.TagName })
                    .IsUnique(); // Prevent duplicate tags on same appointment

                entity.Property(t => t.TagName)
                    .HasMaxLength(50)
                    .IsRequired();
            });

            // Dev 6: CancellationCommand (Command Pattern)
            modelBuilder.Entity<CancellationCommand>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.Appointment)
                    .WithMany(a => a.CancellationCommands)
                    .HasForeignKey(c => c.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.CancelledByUser)
                    .WithMany()
                    .HasForeignKey(c => c.CancelledByUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(c => c.PreviousState)
                    .HasMaxLength(20)
                    .IsRequired();
            });
        }
    }
}