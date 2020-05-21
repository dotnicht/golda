using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.CryptoService.Infrastructure.Persistence.Migrations
{
    public partial class Confirmations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Confimations",
                table: "Transactions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confimations",
                table: "Transactions");
        }
    }
}
