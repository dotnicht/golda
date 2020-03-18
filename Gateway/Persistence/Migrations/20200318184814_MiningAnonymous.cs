using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.Gateway.Persistence.Migrations
{
    public partial class MiningAnonymous : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnomymous",
                table: "MiningRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "MiningRequests",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "MiningRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnomymous",
                table: "MiningRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
