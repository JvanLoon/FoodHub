using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodHub.Persistence.Migrations
{
    /// <summary>
    /// Drops the two demo accounts that InitialCreate seeded via HasData
    /// (admin@foodhub.local / user@foodhub.local) now that IdentitySeed is gone.
    ///
    /// Hand-edited after scaffolding, in two places:
    ///
    /// 1. The generated version also deleted the three rows from AspNetRoles. It
    ///    should not. Those role ids are referenced by AspNetUserRoles for REAL
    ///    accounts too, and the FK cascades — deploying that would have quietly
    ///    stripped the Admin role from every administrator on an existing database.
    ///    The roles now live outside the model and are ensured on every boot by
    ///    IdentityBootstrapExtensions, so leaving the rows in place is correct.
    ///
    /// 2. Down() was generated as an InsertData that re-added both password hashes.
    ///    Restoring known credentials is not a rollback anyone wants, and it would
    ///    keep those hashes in the committed source. It is deliberately a no-op.
    ///
    /// Note: recipes and meal-plan entries reference users by a plain string column
    /// with no foreign key, so anything the demo admin owned on an existing database
    /// survives this migration as an orphan. Fresh deployments have nothing to orphan.
    /// deploy/sql/remove-seeded-identity.sql refuses to run in that situation; this
    /// migration cannot, so check before applying it to a database you care about.
    /// </summary>
    public partial class RemoveSeededIdentityData : Migration
    {
        private const string AdminUserId = "c2f0b2b0-0000-0000-0000-000000000001";
        private const string UserUserId = "c2f0b2b0-0000-0000-0000-000000000002";

        private const string AdminRoleId = "b1e9a1a0-0000-0000-0000-000000000001";
        private const string ModeratorRoleId = "b1e9a1a0-0000-0000-0000-000000000002";
        private const string UserRoleId = "b1e9a1a0-0000-0000-0000-000000000003";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var (roleId, userId) in new[]
                     {
                         (AdminRoleId, AdminUserId),
                         (ModeratorRoleId, AdminUserId),
                         (UserRoleId, AdminUserId),
                         (UserRoleId, UserUserId)
                     })
            {
                migrationBuilder.DeleteData(
                    table: "AspNetUserRoles",
                    keyColumns: new[] { "RoleId", "UserId" },
                    keyValues: new object[] { roleId, userId });
            }

            // AspNetUserClaims/Logins/Tokens cascade from AspNetUsers, so the two
            // deletes below are enough to remove every trace of these accounts.
            migrationBuilder.DeleteData(table: "AspNetUsers", keyColumn: "Id", keyValue: AdminUserId);
            migrationBuilder.DeleteData(table: "AspNetUsers", keyColumn: "Id", keyValue: UserUserId);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty — see the class summary. Rolling back must not
            // recreate accounts whose passwords are published in this repo's history.
        }
    }
}
