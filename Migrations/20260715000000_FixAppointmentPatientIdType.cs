using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Migrations
{
    /// <inheritdoc />
    public partial class FixAppointmentPatientIdType : Migration
    {
        /// <summary>
        /// The original model had Appointment.PatientId as an int with no real relationship
        /// to AspNetUsers (whose Id is a string). EF silently created a *second*, hidden
        /// column ("PatientId1") to satisfy the User.Appointments navigation, which the API
        /// never populated - so every appointment's Patient was always null in practice.
        /// This migration removes the dead int column/shadow FK and makes PatientId a proper
        /// string foreign key to AspNetUsers.Id.
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_PatientId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId_AppointmentDate_AppointmentTime",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Appointments");

            // Old int PatientId values never pointed at a real AspNetUsers.Id, so there is
            // nothing meaningful to preserve - drop it and add the real string column.
            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "PatientId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId_AppointmentDate_AppointmentTime",
                table: "Appointments",
                columns: new[] { "PatientId", "AppointmentDate", "AppointmentTime" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_PatientId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId_AppointmentDate_AppointmentTime",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PatientId1",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId_AppointmentDate_AppointmentTime",
                table: "Appointments",
                columns: new[] { "PatientId", "AppointmentDate", "AppointmentTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId1",
                table: "Appointments",
                column: "PatientId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_PatientId1",
                table: "Appointments",
                column: "PatientId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
