using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kinematik.EntityFramework.Migrations
{
    public partial class AddedHallLayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HallLayoutItems",
                columns: table => new
                {
                    RowID = table.Column<int>(type: "int", nullable: false),
                    ColID = table.Column<int>(type: "int", nullable: false),
                    HallID = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HallLayoutItems", x => new { x.RowID, x.ColID });
                    table.ForeignKey(
                        name: "FK_HallLayoutItems_Halls_HallID",
                        column: x => x.HallID,
                        principalTable: "Halls",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HallLayoutItems_HallID",
                table: "HallLayoutItems",
                column: "HallID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HallLayoutItems");
        }
    }
}
