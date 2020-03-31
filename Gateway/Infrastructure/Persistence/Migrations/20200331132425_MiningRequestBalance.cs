using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Migrations
{
    public partial class MiningRequestBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Transactions",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal (18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal (18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TokenAmount",
                table: "Promotions",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal (18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyAmount",
                table: "Promotions",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal (18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MiningRequests",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal (18,8)");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "MiningRequests",
                type: "decimal(18,8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ExchangeOperations",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal (18,8)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "MiningRequests");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Transactions",
                type: "decimal (18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "decimal (18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TokenAmount",
                table: "Promotions",
                type: "decimal (18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyAmount",
                table: "Promotions",
                type: "decimal (18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "MiningRequests",
                type: "decimal (18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "ExchangeOperations",
                type: "decimal (18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");
        }
    }
}
