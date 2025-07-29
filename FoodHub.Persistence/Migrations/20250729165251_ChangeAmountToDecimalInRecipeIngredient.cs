using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAmountToDecimalInRecipeIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("ALTER TABLE [RecipeIngredients] DROP CONSTRAINT [CK_RecipeIngredient_Amount];");

			migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RecipeIngredients",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

			migrationBuilder.Sql("ALTER TABLE [RecipeIngredients] ADD CONSTRAINT [CK_RecipeIngredient_Amount] CHECK ([Amount] > 0);");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "RecipeIngredients",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
