using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Abocar.Data.Migrations
{
    /// <inheritdoc />
    public partial class GeneralEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "commission",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDeliveryDate",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "commission",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryDate",
                table: "Order");
        }
    }
}
