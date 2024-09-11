using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Abocar.Data.Migrations
{
    /// <inheritdoc />
    public partial class _11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "ProductImages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_CartItemId",
                table: "ProductImages",
                column: "CartItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_CartItems_CartItemId",
                table: "ProductImages",
                column: "CartItemId",
                principalTable: "CartItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_CartItems_CartItemId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_CartItemId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "ProductImages");
        }
    }
}
