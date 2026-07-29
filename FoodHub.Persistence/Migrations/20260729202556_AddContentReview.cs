using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <summary>
    /// Introduces moderated content: recipes and catalog ingredients are now invisible to
    /// everyone but their author until a moderator approves them.
    ///
    /// Hand-edited after scaffolding to add the backfill below. The new columns land with
    /// defaultValue: false, which is right for every row written from here on but wrong for
    /// every row that already exists — without the backfill this migration would hide the
    /// entire live recipe library from everyone except whoever happened to create each one,
    /// and dump all of it into the review queue. The UPDATEs approve the existing data in the
    /// same migration that adds the flag, so the deploy is a no-op for current users.
    ///
    /// RecipeItems is backfilled for a second reason: an unreviewed line is rendered as
    /// "changed" by the review screen, so leaving the existing lines at false would make every
    /// pre-existing recipe look wholly rewritten the first time it is edited.
    ///
    /// FirstApprovedDate is backfilled to each row's own CreatedDate — the truthful "has been
    /// public since" for rows that predate review, and enough to keep them out of the queue's
    /// first-submission bucket. Ingredients.CreatedByUserId is deliberately left empty on
    /// existing rows: they are approved, so nothing reads their author, and inventing one would
    /// misattribute the catalog.
    /// </summary>
    public partial class AddContentReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstApprovedDate",
                table: "Recipes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "Recipes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "RecipeItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Ingredients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstApprovedDate",
                table: "Ingredients",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "Ingredients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // --- Backfill: everything that predates review is already approved. ---
            // Runs before the partial indexes below so they are built over final data.
            migrationBuilder.Sql(
                "UPDATE \"Recipes\" SET \"IsReviewed\" = true, \"FirstApprovedDate\" = \"CreatedDate\";");
            migrationBuilder.Sql("UPDATE \"RecipeItems\" SET \"IsReviewed\" = true;");
            migrationBuilder.Sql(
                "UPDATE \"Ingredients\" SET \"IsReviewed\" = true, \"FirstApprovedDate\" = \"CreatedDate\";");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_IsReviewed_Pending",
                table: "Recipes",
                column: "IsReviewed",
                filter: "\"IsReviewed\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_CreatedByUserId",
                table: "Ingredients",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_IsReviewed_Pending",
                table: "Ingredients",
                column: "IsReviewed",
                filter: "\"IsReviewed\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recipes_IsReviewed_Pending",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_CreatedByUserId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_IsReviewed_Pending",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "FirstApprovedDate",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "RecipeItems");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "FirstApprovedDate",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "Ingredients");
        }
    }
}
