using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedIndexRecipeItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_RecipeItem_RecipeId_Name",
                table: "RecipeItems",
                newName: "UX_RecipeItem_RecipeId_IngredientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_RecipeItem_RecipeId_IngredientId",
                table: "RecipeItems",
                newName: "UX_RecipeItem_RecipeId_Name");
        }
    }
}
