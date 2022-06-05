using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kinematik.EntityFramework.Migrations
{
    public partial class AddedIsPayedForfieldforBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayedFor",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPayedFor",
                table: "Bookings");
        }
    }
}
