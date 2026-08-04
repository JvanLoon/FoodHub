using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <summary>
    /// Data-only: no schema change, so Up is hand-written.
    ///
    /// Every account enabled through the admin UI was written with LockoutEnabled = false,
    /// because that flag was being used to mean "disabled" rather than "may be locked out". With
    /// it off, UserManager.IsLockedOutAsync returns false without looking any further, so failed
    /// sign-ins were counted and never acted on — no brute-force protection on any working
    /// account. The code no longer writes false anywhere; this repairs the rows already stored.
    /// </summary>
    public partial class EnableLockoutForAllUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Does not touch LockoutEnd: an account part-way through a lockout, or disabled by
            // setting it to MaxValue, should stay exactly as it is.
            migrationBuilder.Sql(
                """
                UPDATE "AspNetUsers" SET "LockoutEnabled" = TRUE WHERE "LockoutEnabled" = FALSE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Deliberately empty. A true inverse would switch lockout back off, and reopening
            // that hole is not something a rollback should do quietly. Nothing reads the old
            // value, so leaving it on is safe to roll back through.
        }
    }
}
