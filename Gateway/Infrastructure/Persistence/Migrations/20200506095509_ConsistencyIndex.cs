using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Migrations
{
    public partial class ConsistencyIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BalanceRecords_To",
                table: "BalanceRecords",
                column: "To")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BalanceRecords_To",
                table: "BalanceRecords");
        }
    }
}
