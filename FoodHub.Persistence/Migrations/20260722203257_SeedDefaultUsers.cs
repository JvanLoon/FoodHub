using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b1e9a1a0-0000-0000-0000-000000000001", "b1e9a1a0-0000-0000-0000-000000000001", "Admin", "ADMIN" },
                    { "b1e9a1a0-0000-0000-0000-000000000002", "b1e9a1a0-0000-0000-0000-000000000002", "Moderator", "MODERATOR" },
                    { "b1e9a1a0-0000-0000-0000-000000000003", "b1e9a1a0-0000-0000-0000-000000000003", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "c2f0b2b0-0000-0000-0000-000000000001", 0, "c2f0b2b0-0000-0000-0000-000000000001", "admin@foodhub.local", true, false, null, "ADMIN@FOODHUB.LOCAL", "ADMIN@FOODHUB.LOCAL", "AQAAAAIAAYagAAAAEI9e1iMqykqith/CN4pGUK+H1zrVfp5ERttCqYZzKAMBWKvik5W8jJt1ukNyZIw+6g==", null, false, "c2f0b2b0-0000-0000-0000-000000000001", false, "admin@foodhub.local" },
                    { "c2f0b2b0-0000-0000-0000-000000000002", 0, "c2f0b2b0-0000-0000-0000-000000000002", "user@foodhub.local", true, false, null, "USER@FOODHUB.LOCAL", "USER@FOODHUB.LOCAL", "AQAAAAIAAYagAAAAEMQmfpGbK2nWDteIHDEYhwcLUe0UDntdRwKgvrKQ5Jatcian7eTgcM4yD6Q10kKcHg==", null, false, "c2f0b2b0-0000-0000-0000-000000000002", false, "user@foodhub.local" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "b1e9a1a0-0000-0000-0000-000000000001", "c2f0b2b0-0000-0000-0000-000000000001" },
                    { "b1e9a1a0-0000-0000-0000-000000000002", "c2f0b2b0-0000-0000-0000-000000000001" },
                    { "b1e9a1a0-0000-0000-0000-000000000003", "c2f0b2b0-0000-0000-0000-000000000001" },
                    { "b1e9a1a0-0000-0000-0000-000000000003", "c2f0b2b0-0000-0000-0000-000000000002" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b1e9a1a0-0000-0000-0000-000000000001", "c2f0b2b0-0000-0000-0000-000000000001" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b1e9a1a0-0000-0000-0000-000000000002", "c2f0b2b0-0000-0000-0000-000000000001" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b1e9a1a0-0000-0000-0000-000000000003", "c2f0b2b0-0000-0000-0000-000000000001" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b1e9a1a0-0000-0000-0000-000000000003", "c2f0b2b0-0000-0000-0000-000000000002" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1e9a1a0-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1e9a1a0-0000-0000-0000-000000000002");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1e9a1a0-0000-0000-0000-000000000003");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c2f0b2b0-0000-0000-0000-000000000001");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c2f0b2b0-0000-0000-0000-000000000002");
        }
    }
}
