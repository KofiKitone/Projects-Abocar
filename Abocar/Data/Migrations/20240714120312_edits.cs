using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Abocar.Data.Migrations
{
    /// <inheritdoc />
    public partial class edits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "LocalPickUp");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "LocalPickUp");

            migrationBuilder.RenameColumn(
                name: "AddressLine2",
                table: "LocalPickUp",
                newName: "AddressLine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddressLine",
                table: "LocalPickUp",
                newName: "AddressLine2");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "LocalPickUp",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "LocalPickUp",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
