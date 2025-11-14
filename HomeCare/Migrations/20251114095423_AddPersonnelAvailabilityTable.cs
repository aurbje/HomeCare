using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonnelAvailabilityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonnelAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonnelId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelAppointments_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelAppointments_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonnelAppointments_Users_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonnelId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelAvailabilities_Users_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAppointments_CategoryId",
                table: "PersonnelAppointments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAppointments_ClientId",
                table: "PersonnelAppointments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAppointments_PersonnelId",
                table: "PersonnelAppointments",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelAvailabilities_PersonnelId",
                table: "PersonnelAvailabilities",
                column: "PersonnelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonnelAppointments");

            migrationBuilder.DropTable(
                name: "PersonnelAvailabilities");
        }
    }
}
