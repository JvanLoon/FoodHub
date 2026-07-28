-- Removes the two accounts that used to be baked into the InitialCreate migration
-- (admin@foodhub.local / user@foodhub.local) from a database that was created
-- BEFORE the seed was dropped from the model.
--
-- You do not need this on a database created from scratch on this branch — the
-- RemoveSeededIdentityData migration already deletes those rows. It exists for an
-- existing environment where the rows are present and you would rather not wait
-- for the migration, or where they were edited so the migration's DeleteData no
-- longer matches them.
--
-- Run it against the application database:
--   psql "$CONNECTION_STRING" -v ON_ERROR_STOP=1 -f remove-seeded-identity.sql
--
-- Safe to run twice. It touches ONLY the two fixed seed ids below, so accounts you
-- created yourself are never affected — including one that reuses the same email.

BEGIN;

-- Fixed ids from the old IdentitySeed. Matching on id rather than email means a
-- real account that happens to use admin@foodhub.local survives untouched.
CREATE TEMP TABLE seeded_user_ids (id text PRIMARY KEY) ON COMMIT DROP;
INSERT INTO seeded_user_ids (id)
VALUES ('c2f0b2b0-0000-0000-0000-000000000001'),  -- admin@foodhub.local
       ('c2f0b2b0-0000-0000-0000-000000000002');  -- user@foodhub.local

-- Recipes carry the creating user's id as a plain string, with no FK to the
-- Identity tables, so deleting the account would silently orphan its recipes.
-- Refuse to continue if that would happen: reassigning them is a decision for a
-- human, not for this script.
DO $$
DECLARE
    orphan_count integer;
BEGIN
    SELECT count(*)
      INTO orphan_count
      FROM "Recipes" r
      JOIN seeded_user_ids s ON s.id = r."CreatedByUserId";

    IF orphan_count > 0 THEN
        RAISE EXCEPTION
            'Refusing to delete: % recipe(s) are owned by a seeded account. '
            'Reassign them first, e.g. UPDATE "Recipes" SET "CreatedByUserId" = ''<your-user-id>'' '
            'WHERE "CreatedByUserId" IN (SELECT id FROM seeded_user_ids);', orphan_count;
    END IF;
END $$;

-- Meal plan entries are per-user and disposable, so they go with the account.
DELETE FROM "MealPlanEntries" WHERE "UserId" IN (SELECT id FROM seeded_user_ids);

-- RecipeBlackList.UserId is a Guid on the entity (so `uuid` here) while every other
-- user reference is the Identity string key. The seed ids are valid UUIDs, so the
-- cast is safe — but see "Known issues" in deployment.md, this mismatch is a bug.
DELETE FROM "RecipeBlackLists" WHERE "UserId" IN (SELECT id::uuid FROM seeded_user_ids);

DELETE FROM "AspNetUserRoles"  WHERE "UserId" IN (SELECT id FROM seeded_user_ids);
DELETE FROM "AspNetUserClaims" WHERE "UserId" IN (SELECT id FROM seeded_user_ids);
DELETE FROM "AspNetUserLogins" WHERE "UserId" IN (SELECT id FROM seeded_user_ids);
DELETE FROM "AspNetUserTokens" WHERE "UserId" IN (SELECT id FROM seeded_user_ids);
DELETE FROM "AspNetUsers"      WHERE "Id"     IN (SELECT id FROM seeded_user_ids);

COMMIT;

-- The Admin/Moderator/User ROLES are intentionally left in place: the API's
-- authorization policies name them, and the identity bootstrapper recreates them
-- on every boot anyway. Only the accounts are removed.

\echo 'Seeded accounts removed. Verify with:'
\echo '  SELECT "Id", "Email" FROM "AspNetUsers";'
