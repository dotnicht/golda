using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Migrations
{
    public partial class MultiPromotionMining : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Promotions_MiningRequestId",
                table: "Promotions");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_MiningRequestId",
                table: "Promotions",
                column: "MiningRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Promotions_MiningRequestId",
                table: "Promotions");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_MiningRequestId",
                table: "Promotions",
                column: "MiningRequestId",
                unique: true);
        }
    }
}
