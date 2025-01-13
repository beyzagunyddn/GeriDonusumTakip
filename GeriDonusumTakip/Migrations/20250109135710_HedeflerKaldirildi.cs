using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeriDonusumTakip.Migrations
{
    /// <inheritdoc />
    public partial class HedeflerKaldirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciHedefler");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KullaniciHedefler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GerceklesenMiktar = table.Column<double>(type: "float", nullable: false),
                    HedefMiktar = table.Column<double>(type: "float", nullable: false),
                    HedefTur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tamamlandi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciHedefler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciHedefler_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciHedefler_KullaniciId",
                table: "KullaniciHedefler",
                column: "KullaniciId");
        }
    }
}
