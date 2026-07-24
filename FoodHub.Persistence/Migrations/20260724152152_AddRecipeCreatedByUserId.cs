using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeCreatedByUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Recipes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CreatedByUserId",
                table: "Recipes",
                column: "CreatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_CreatedByUserId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Recipes");
        }
    }
}
