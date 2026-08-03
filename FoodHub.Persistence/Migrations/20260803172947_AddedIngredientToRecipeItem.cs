using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedIngredientToRecipeItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_RecipeItem_RecipeId_Name",
                table: "RecipeItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RecipeItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "IngredientId",
                table: "RecipeItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_RecipeItem_RecipeId_Name",
                table: "RecipeItems",
                columns: new[] { "RecipeId", "IngredientId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_RecipeItem_RecipeId_Name",
                table: "RecipeItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "IngredientId",
                table: "RecipeItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RecipeItems",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UX_RecipeItem_RecipeId_Name",
                table: "RecipeItems",
                columns: new[] { "RecipeId", "Name" },
                unique: true);
        }
    }
}
