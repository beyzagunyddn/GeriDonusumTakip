using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeriDonusumTakip.Migrations
{
    /// <inheritdoc />
    public partial class GeriDonusumModelGuncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeriDonusumler_AspNetUsers_KullaniciId",
                table: "GeriDonusumler");

            migrationBuilder.RenameColumn(
                name: "Aciklama",
                table: "GeriDonusumler",
                newName: "Notlar");

            migrationBuilder.AlterColumn<string>(
                name: "KullaniciId",
                table: "GeriDonusumler",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Tur",
                table: "GeriDonusumler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_GeriDonusumler_AspNetUsers_KullaniciId",
                table: "GeriDonusumler",
                column: "KullaniciId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeriDonusumler_AspNetUsers_KullaniciId",
                table: "GeriDonusumler");

            migrationBuilder.DropColumn(
                name: "Tur",
                table: "GeriDonusumler");

            migrationBuilder.RenameColumn(
                name: "Notlar",
                table: "GeriDonusumler",
                newName: "Aciklama");

            migrationBuilder.AlterColumn<string>(
                name: "KullaniciId",
                table: "GeriDonusumler",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GeriDonusumler_AspNetUsers_KullaniciId",
                table: "GeriDonusumler",
                column: "KullaniciId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
