using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kinematik.EntityFramework.Migrations
{
    public partial class RenamedHallLayoutItemTypetoSeatType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "HallLayoutItems",
                newName: "SeatType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeatType",
                table: "HallLayoutItems",
                newName: "Type");
        }
    }
}
