using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Migrations
{
    public partial class RemoveTxSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(string));

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

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
