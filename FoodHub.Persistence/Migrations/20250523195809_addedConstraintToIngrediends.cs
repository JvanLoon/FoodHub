using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addedConstraintToIngrediends : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_RecipeId",
                table: "RecipeIngredients");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecipeIngredient_Amount",
                table: "RecipeIngredients");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecipeIngredient_IngredientAmount",
                table: "RecipeIngredients");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ingredients",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "UX_RecipeIngredient_RecipeId_IngredientId",
                table: "RecipeIngredients",
                columns: new[] { "RecipeId", "IngredientId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecipeIngredient_Amount",
                table: "RecipeIngredients",
                sql: "Amount > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_RecipeIngredient_IngredientAmount",
                table: "RecipeIngredients",
                sql: "IngredientAmount > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_RecipeIngredient_RecipeId_IngredientId",
                table: "RecipeIngredients");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecipeIngredient_Amount",
                table: "RecipeIngredients");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RecipeIngredient_IngredientAmount",
                table: "RecipeIngredients");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_RecipeId",
                table: "RecipeIngredients",
                column: "RecipeId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ReceptIngredient_Amount",
                table: "RecipeIngredients",
                sql: "Amount > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ReceptIngredient_IngredientAmount",
                table: "RecipeIngredients",
                sql: "IngredientAmount > 0");
        }
    }
}
