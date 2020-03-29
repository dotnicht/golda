using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.CryptoService.Infrastructure.Persistence.Migrations
{
    public partial class UnsignenLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "RawAmount",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LastBlock",
                table: "Addresses",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Index",
                table: "Addresses",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "GeneratedBlock",
                table: "Addresses",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "Addresses",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Addresses",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RawAmount",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<string>(
                name: "Hash",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<long>(
                name: "LastBlock",
                table: "Addresses",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "Index",
                table: "Addresses",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "GeneratedBlock",
                table: "Addresses",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "Currency",
                table: "Addresses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Balance",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
