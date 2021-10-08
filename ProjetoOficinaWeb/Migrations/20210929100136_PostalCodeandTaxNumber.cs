using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjetoOficinaWeb.Migrations
{
    public partial class PostalCodeandTaxNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostalCode",
                table: "AspNetUsers",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TaxNumber",
                table: "AspNetUsers",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "AspNetUsers");
        }
    }
}
