using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCancelledColumnOnSaleItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "OccurredAt",
                table: "Sales",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 26, 0, 44, 58, 906, DateTimeKind.Utc).AddTicks(4492),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 4, 21, 3, 20, 5, 612, DateTimeKind.Utc).AddTicks(2374));

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "SaleItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "SaleItems");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OccurredAt",
                table: "Sales",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 21, 3, 20, 5, 612, DateTimeKind.Utc).AddTicks(2374),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 4, 26, 0, 44, 58, 906, DateTimeKind.Utc).AddTicks(4492));
        }
    }
}
