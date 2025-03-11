using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NewRecept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recepts_ReceptId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_ReceptId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Prio",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "ReceptId",
                table: "Ingredients");

            migrationBuilder.CreateTable(
                name: "IngredientAmountTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientAmountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceptIngredients",
                columns: table => new
                {
                    ReceptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    IngredientAmount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceptIngredients", x => new { x.ReceptId, x.IngredientId });
                    table.CheckConstraint("CK_ReceptIngredient_Amount", "Amount > 0");
                    table.CheckConstraint("CK_ReceptIngredient_IngredientAmount", "IngredientAmount > 0");
                    table.ForeignKey(
                        name: "FK_ReceptIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceptIngredients_Recepts_ReceptId",
                        column: x => x.ReceptId,
                        principalTable: "Recepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IngredientAmountTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "None" },
                    { 1, "Gram" },
                    { 2, "Kilogram" },
                    { 3, "Liter" },
                    { 4, "Milliliter" },
                    { 5, "Piece" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceptIngredients_IngredientId",
                table: "ReceptIngredients",
                column: "IngredientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientAmountTypes");

            migrationBuilder.DropTable(
                name: "ReceptIngredients");

            migrationBuilder.AddColumn<string>(
                name: "Amount",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Prio",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceptId",
                table: "Ingredients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_ReceptId",
                table: "Ingredients",
                column: "ReceptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recepts_ReceptId",
                table: "Ingredients",
                column: "ReceptId",
                principalTable: "Recepts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
