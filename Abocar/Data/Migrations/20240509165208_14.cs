using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Abocar.Data.Migrations
{
    /// <inheritdoc />
    public partial class _14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EstimatedDeliveryTime",
                table: "DeliveryOption",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_OrderItemId",
                table: "ProductImages",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_OrderItems_OrderItemId",
                table: "ProductImages",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_OrderItems_OrderItemId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_OrderItemId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "ProductImages");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EstimatedDeliveryTime",
                table: "DeliveryOption",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
