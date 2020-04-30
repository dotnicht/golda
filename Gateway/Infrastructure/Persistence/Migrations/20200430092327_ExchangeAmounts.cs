using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Migrations
{
    public partial class ExchangeAmounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "ExchangeOperations",
                newName: "QuoteAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "BaseAmount",
                table: "ExchangeOperations",
                type: "decimal(18,8)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseAmount",
                table: "ExchangeOperations");

            migrationBuilder.RenameColumn(
                name: "QuoteAmount",
                table: "ExchangeOperations",
                newName: "Amount");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Transactions",
                type: "decimal(18,8)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
