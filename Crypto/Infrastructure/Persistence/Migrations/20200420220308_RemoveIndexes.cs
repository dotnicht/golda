using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.CryptoService.Infrastructure.Persistence.Migrations
{
    public partial class RemoveIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneratedBlock",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "LastBlock",
                table: "Addresses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GeneratedBlock",
                table: "Addresses",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LastBlock",
                table: "Addresses",
                type: "decimal(20,0)",
                nullable: true);
        }
    }
}
