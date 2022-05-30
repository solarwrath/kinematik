using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kinematik.EntityFramework.Migrations
{
    public partial class FixingincorrectPKforHallLayoutItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HallLayoutItems",
                table: "HallLayoutItems");

            migrationBuilder.DropIndex(
                name: "IX_HallLayoutItems_HallID",
                table: "HallLayoutItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HallLayoutItems",
                table: "HallLayoutItems",
                columns: new[] { "HallID", "RowID", "ColumnID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HallLayoutItems",
                table: "HallLayoutItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HallLayoutItems",
                table: "HallLayoutItems",
                columns: new[] { "RowID", "ColumnID" });

            migrationBuilder.CreateIndex(
                name: "IX_HallLayoutItems_HallID",
                table: "HallLayoutItems",
                column: "HallID");
        }
    }
}
