using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class movedPropertyShouldBeAddedToShoppingCartToIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShouldBeAddedToShoppingCart",
                table: "RecipeIngredients");

            migrationBuilder.AddColumn<bool>(
                name: "ShouldBeAddedToShoppingCart",
                table: "Ingredients",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShouldBeAddedToShoppingCart",
                table: "Ingredients");

            migrationBuilder.AddColumn<bool>(
                name: "ShouldBeAddedToShoppingCart",
                table: "RecipeIngredients",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
