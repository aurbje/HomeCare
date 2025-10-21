using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AvailableDates",
                columns: new[] { "Id", "Date" },
                values: new object[,]
                {
                    { 3, new DateTime(2025, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2025, 12, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2025, 12, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2025, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "Id", "AvailableDateId", "IsBooked", "Slot" },
                values: new object[,]
                {
                    { 6, 3, false, "09:00-10:00" },
                    { 7, 3, false, "10:00-11:00" },
                    { 8, 4, false, "11:00-12:00" },
                    { 9, 5, false, "13:00-14:00" },
                    { 10, 6, false, "14:00-15:00" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AvailableDates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AvailableDates",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AvailableDates",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AvailableDates",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}