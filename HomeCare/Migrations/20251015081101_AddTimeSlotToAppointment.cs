using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeSlotToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomCategory",
                table: "Appointments",
                newName: "TimeSlot");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeSlot",
                table: "Appointments",
                newName: "CustomCategory");
        }
    }
}