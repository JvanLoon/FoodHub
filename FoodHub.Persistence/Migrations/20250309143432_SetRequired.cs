using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SetRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recepts_ReceptId",
                table: "Ingredients");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Recepts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Prio",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Recepts_Name",
                table: "Recepts",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recepts_ReceptId",
                table: "Ingredients",
                column: "ReceptId",
                principalTable: "Recepts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Recepts_ReceptId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Recepts_Name",
                table: "Recepts");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Recepts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Prio",
                table: "Ingredients",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Recepts_ReceptId",
                table: "Ingredients",
                column: "ReceptId",
                principalTable: "Recepts",
                principalColumn: "Id");
        }
    }
}
