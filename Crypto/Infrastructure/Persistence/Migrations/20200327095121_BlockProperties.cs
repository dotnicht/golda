using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.CryptoService.Infrastructure.Persistence.Migrations
{
    public partial class BlockProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Block",
                table: "Addresses");

            migrationBuilder.AddColumn<DateTime>(
                name: "Confirmed",
                table: "Transactions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "GeneratedBlock",
                table: "Addresses",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LastBlock",
                table: "Addresses",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "GeneratedBlock",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "LastBlock",
                table: "Addresses");

            migrationBuilder.AddColumn<long>(
                name: "Block",
                table: "Addresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
