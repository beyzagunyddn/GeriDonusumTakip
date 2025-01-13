using Microsoft.EntityFrameworkCore.Migrations;

public partial class KullaniciRozetleriEklendi : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Rozetler",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "ToplamPuan",
            table: "AspNetUsers",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

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