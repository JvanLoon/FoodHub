using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReworkRecipeIngredientSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add the snapshot columns (Name defaults to "" so the NOT NULL add succeeds
            //    on existing rows; it is backfilled from the linked Ingredient below).
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RecipeIngredients",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ShouldBeAddedToShoppingCart",
                table: "RecipeIngredients",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // 2. Backfill the snapshot from the catalog Ingredient before the link is dropped.
            migrationBuilder.Sql(@"
                UPDATE ri
                SET ri.Name = i.Name,
                    ri.ShouldBeAddedToShoppingCart = i.ShouldBeAddedToShoppingCart
                FROM RecipeIngredients ri
                INNER JOIN Ingredients i ON ri.IngredientId = i.Id;");

            // 3. Drop the link to Ingredient now that the name/flag are snapshotted.
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientId",
                table: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_IngredientId",
                table: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "UX_RecipeIngredient_RecipeId_IngredientId",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "IngredientId",
                table: "RecipeIngredients");

            // 4. Enforce uniqueness by name (after backfill, so real names — not "" — are indexed).
            migrationBuilder.CreateIndex(
                name: "UX_RecipeIngredient_RecipeId_Name",
                table: "RecipeIngredients",
                columns: new[] { "RecipeId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_RecipeIngredient_RecipeId_Name",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "ShouldBeAddedToShoppingCart",
                table: "RecipeIngredients");

            migrationBuilder.AddColumn<Guid>(
                name: "IngredientId",
                table: "RecipeIngredients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientId",
                table: "RecipeIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "UX_RecipeIngredient_RecipeId_IngredientId",
                table: "RecipeIngredients",
                columns: new[] { "RecipeId", "IngredientId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Ingredients_IngredientId",
                table: "RecipeIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
