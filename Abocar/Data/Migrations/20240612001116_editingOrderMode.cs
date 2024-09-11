using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Abocar.Data.Migrations
{
    /// <inheritdoc />
    public partial class editingOrderMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Address_AddressId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_AddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveryOptionId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentOptionId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryOption",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentOption",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryOption",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentOption",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryOptionId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentOptionId",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Order_AddressId",
                table: "Order",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Address_AddressId",
                table: "Order",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
