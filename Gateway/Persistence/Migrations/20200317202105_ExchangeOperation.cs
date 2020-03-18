using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Binebase.Exchange.Gateway.Persistence.Migrations
{
    public partial class ExchangeOperation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TokenAmount",
                table: "Promotions",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyAmount",
                table: "Promotions",
                type: "decimal(18,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "MiningRequestId",
                table: "Promotions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsAnomymous",
                table: "MiningRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ExchangeOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    LastModifiedBy = table.Column<Guid>(nullable: false),
                    Base = table.Column<string>(nullable: true),
                    Quote = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeOperations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_MiningRequestId",
                table: "Promotions",
                column: "MiningRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_MiningRequests_MiningRequestId",
                table: "Promotions",
                column: "MiningRequestId",
                principalTable: "MiningRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_MiningRequests_MiningRequestId",
                table: "Promotions");

            migrationBuilder.DropTable(
                name: "ExchangeOperations");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_MiningRequestId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "MiningRequestId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "IsAnomymous",
                table: "MiningRequests");

            migrationBuilder.AlterColumn<decimal>(
                name: "TokenAmount",
                table: "Promotions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyAmount",
                table: "Promotions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");
        }
    }
}
