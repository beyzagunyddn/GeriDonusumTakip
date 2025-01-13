using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeriDonusumTakip.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciRozetleriEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rozetler",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "ToplamPuan",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rozetler",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ToplamPuan",
                table: "AspNetUsers");
        }
    }
}
