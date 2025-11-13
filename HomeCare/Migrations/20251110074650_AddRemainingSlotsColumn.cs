using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingSlotsColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingSlots",
                table: "AvailableDates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AvailableDates",
                keyColumn: "Id",
                keyValue: 1,
                column: "RemainingSlots",
                value: 0);

            migrationBuilder.UpdateData(
                table: "AvailableDates",
                keyColumn: "Id",
                keyValue: 2,
                column: "RemainingSlots",
                value: 0);

            migrationBuilder.UpdateData(
                table: "AvailableDates",
                keyColumn: "Id",
                keyValue: 3,
                column: "RemainingSlots",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingSlots",
                table: "AvailableDates");
        }
    }
}
