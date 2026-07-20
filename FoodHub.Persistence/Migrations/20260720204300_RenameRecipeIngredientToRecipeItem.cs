using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameRecipeIngredientToRecipeItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename the table, its keys, constraints and index in place. EF scaffolds
            // a drop/create for an entity rename, which would discard all rows; renaming
            // preserves the existing data. Constraint names are brought in line with the
            // conventions the model snapshot now expects (PK_RecipeItems, etc.).
            migrationBuilder.RenameTable(
                name: "RecipeIngredients",
                newName: "RecipeItems");

            migrationBuilder.Sql("EXEC sp_rename 'PK_RecipeIngredients', 'PK_RecipeItems', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'FK_RecipeIngredients_Recipes_RecipeId', 'FK_RecipeItems_Recipes_RecipeId', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'CK_RecipeIngredient_Amount', 'CK_RecipeItem_Amount', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'CK_RecipeIngredient_IngredientAmount', 'CK_RecipeItem_IngredientAmount', 'OBJECT';");

            migrationBuilder.RenameIndex(
                name: "UX_RecipeIngredient_RecipeId_Name",
                newName: "UX_RecipeItem_RecipeId_Name",
                table: "RecipeItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UX_RecipeItem_RecipeId_Name",
                newName: "UX_RecipeIngredient_RecipeId_Name",
                table: "RecipeItems");

            migrationBuilder.Sql("EXEC sp_rename 'CK_RecipeItem_IngredientAmount', 'CK_RecipeIngredient_IngredientAmount', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'CK_RecipeItem_Amount', 'CK_RecipeIngredient_Amount', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'FK_RecipeItems_Recipes_RecipeId', 'FK_RecipeIngredients_Recipes_RecipeId', 'OBJECT';");
            migrationBuilder.Sql("EXEC sp_rename 'PK_RecipeItems', 'PK_RecipeIngredients', 'OBJECT';");

            migrationBuilder.RenameTable(
                name: "RecipeItems",
                newName: "RecipeIngredients");
        }
    }
}
