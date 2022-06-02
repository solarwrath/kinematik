using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kinematik.EntityFramework.Migrations
{
    public partial class IntroducedBookingandBookedSeat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionID = table.Column<int>(type: "int", nullable: false),
                    ClientPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bookings_Sessions_SessionID",
                        column: x => x.SessionID,
                        principalTable: "Sessions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookedSeats",
                columns: table => new
                {
                    RowID = table.Column<int>(type: "int", nullable: false),
                    ColumnID = table.Column<int>(type: "int", nullable: false),
                    SessionID = table.Column<int>(type: "int", nullable: false),
                    BookingID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookedSeats", x => new { x.SessionID, x.RowID, x.ColumnID });
                    table.ForeignKey(
                        name: "FK_BookedSeats_Bookings_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookedSeats_BookingID",
                table: "BookedSeats",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SessionID",
                table: "Bookings",
                column: "SessionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookedSeats");

            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}
