using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <summary>
    /// Links every recipe line back to the catalog entry it came from, so ingredients can be
    /// compared by identity instead of by name.
    ///
    /// Hand-edited after scaffolding to add the two statements between the column and the
    /// foreign key. Without them the column would be added empty, and an empty link is not a
    /// neutral starting point: the find-by-ingredients page matches on it exclusively, so every
    /// existing recipe would drop out of the search until someone re-saved it by hand.
    ///
    /// The first statement creates the catalog entries the recipe library refers to but the
    /// catalog never got — imports and older writes could put a name on a line without ever
    /// registering it. They arrive approved, matching how AddContentReview treated the data that
    /// predates review, and with an empty CreatedByUserId for the same reason: they are public
    /// already, so nothing reads their author, and inventing one would misattribute the catalog.
    ///
    /// The second picks the canonical entry per name and points the lines at it. Ingredients.Name
    /// carries no unique index, so duplicates exist and the choice has to be deterministic:
    /// approved first (an unapproved row is invisible to everyone but its author, and linking a
    /// public recipe to one would make the line unmatchable for everyone else), then oldest.
    /// Names are compared trimmed and case-folded, which is the identity the UI has always
    /// worked with.
    ///
    /// Down() leaves the created catalog entries in place. They are indistinguishable from
    /// hand-added ones by then, and deleting them could take a user's own entry with it.
    /// </summary>
    public partial class AddRecipeItemIngredientLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IngredientId",
                table: "RecipeItems",
                type: "uuid",
                nullable: true);

            // Identifiers are quoted throughout: PostgreSQL folds unquoted ones to lower-case,
            // so "Name" would resolve to a non-existent `name` column.
            migrationBuilder.Sql("""
                INSERT INTO "Ingredients" ("Id", "Name", "ShouldBeAddedToShoppingCart", "CreatedByUserId",
                                          "IsReviewed", "FirstApprovedDate", "CreatedDate", "ModifiedDate")
                SELECT gen_random_uuid(),
                       missing."Name",
                       missing."ShouldBeAddedToShoppingCart",
                       '',
                       TRUE,
                       now(),
                       now(),
                       now()
                FROM (
                    SELECT DISTINCT ON (lower(btrim(ri."Name")))
                           btrim(ri."Name") AS "Name",
                           ri."ShouldBeAddedToShoppingCart"
                    FROM "RecipeItems" ri
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM "Ingredients" i
                        WHERE lower(btrim(i."Name")) = lower(btrim(ri."Name"))
                    )
                    ORDER BY lower(btrim(ri."Name")), ri."CreatedDate", ri."Id"
                ) AS missing;
                """);

            migrationBuilder.Sql("""
                UPDATE "RecipeItems" ri
                SET "IngredientId" = canonical."Id"
                FROM (
                    SELECT DISTINCT ON (lower(btrim(i."Name")))
                           lower(btrim(i."Name")) AS "Key",
                           i."Id"
                    FROM "Ingredients" i
                    ORDER BY lower(btrim(i."Name")), i."IsReviewed" DESC, i."CreatedDate", i."Id"
                ) AS canonical
                WHERE lower(btrim(ri."Name")) = canonical."Key";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeItems_IngredientId",
                table: "RecipeItems",
                column: "IngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeItems_Ingredients_IngredientId",
                table: "RecipeItems",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        /// <remarks>The catalog entries created by Up() are deliberately kept — see the class summary.</remarks>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeItems_Ingredients_IngredientId",
                table: "RecipeItems");

            migrationBuilder.DropIndex(
                name: "IX_RecipeItems_IngredientId",
                table: "RecipeItems");

            migrationBuilder.DropColumn(
                name: "IngredientId",
                table: "RecipeItems");
        }
    }
}
