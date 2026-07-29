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
    /// FirstApprovedDate is backfilled to each row's own CreatedDate. There is no record of a
    /// real approval — these rows predate review entirely — and CreatedDate is both the
    /// truthful "has existed and been public since" and enough to keep them out of the queue's
    /// first-submission bucket. Any non-null value would do; an invented recent timestamp
    /// would read as a moderation that never happened.
    ///
    /// Ingredients.CreatedByUserId is deliberately left empty on existing rows rather than
    /// backfilled to an admin. Those rows are approved, so nothing reads their author — and
    /// inventing one would misattribute the whole catalog.
    /// </summary>
    public partial class AddRecipeAndIngredientReview : Migration
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

            migrationBuilder.CreateTable(
                name: "ReviewRejections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetType = table.Column<int>(type: "integer", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetName = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TargetOwnerUserId = table.Column<string>(type: "text", nullable: false),
                    RejectedByUserId = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TargetDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewRejections", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRejections_TargetOwnerUserId",
                table: "ReviewRejections",
                column: "TargetOwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRejections_TargetType_TargetId",
                table: "ReviewRejections",
                columns: new[] { "TargetType", "TargetId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewRejections");

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
