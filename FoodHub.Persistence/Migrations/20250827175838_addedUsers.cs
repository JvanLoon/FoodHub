using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
					AccessFailedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
					ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
					EmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
					Enabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
					LockoutEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
					LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
					NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
					SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				}
			);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			// Drop the recreated Users table
			migrationBuilder.DropTable(
				name: "Users"
			);
		}
    }
}
