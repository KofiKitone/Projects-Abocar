using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Abocar.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditingDeliveryOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EstimatedDeliveryTime",
                table: "DeliveryOption",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "DeliveryOption",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "DeliveryOption");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EstimatedDeliveryTime",
                table: "DeliveryOption",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
