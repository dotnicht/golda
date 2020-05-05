using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Migrations
{
    public partial class BalanceConsistency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BalanceRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: true),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    Currency = table.Column<string>(nullable: false),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    DebitCount = table.Column<int>(nullable: false),
                    CreditCount = table.Column<int>(nullable: false),
                    StartBalance = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    EndBalance = table.Column<decimal>(type: "decimal(18,8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceRecords");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "AspNetUsers");
        }
    }
}
