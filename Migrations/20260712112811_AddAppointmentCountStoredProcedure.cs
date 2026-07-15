using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentCountStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetAppointmentCountPerDoctor
                AS
                BEGIN
                    SELECT 
                        d.Id AS DoctorId,
                        d.Name AS DoctorName,
                        COUNT(a.AppointmentId) AS TotalAppointments
                    FROM Doctors d
                    LEFT JOIN Appointments a ON a.DoctorId = d.Id
                    GROUP BY d.Id, d.Name
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE GetAppointmentCountPerDoctor");
        }
    }
}