using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Migrations
{
    public partial class RemoveMiningBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "MiningRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "MiningRequests",
                type: "decimal(18,8)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
