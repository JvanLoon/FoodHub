# Feature: Meal Calendar & role-based home

Branch: `feature/calendar` (off `master`).

## Goals

1. **Role-based landing.** Role `User` gets a new simple home page (default at `/`);
   `Admin` keeps the existing stats dashboard. Users can navigate to only Home,
   Recipes, Find recipes, Calendar and User settings — every other page redirects
   to `/` when a user without the required role reaches it by typing the URL.
2. **Meal calendar** at `/calendar` with **week and month views side by side**.
   Each day cell has a **＋** button to add a recipe to eat that day. Entries are
   persisted per user in the database.
3. **Randomize.** Days are multi-selectable. When **more than one** day is
   selected the *Randomize* button enables. It opens a popup where the user may
   optionally list ingredients they want in the picked recipes, then fills the
   selected days with random recipes drawn from the existing recipe library.

## Decisions (confirmed with the user)

- "Generated" recipes = **randomly picked from existing recipes**, optionally
  biased by the desired ingredients. No AI/LLM.
- A day may hold **up to 20 recipes**.
- Randomize popup fields:
  - *Ingredients* (optional) — bias the random pick.
  - *Recipes per day* — logic supports it, but the control is **hidden/disabled
    for now** (defaults to 1).
  - *Overwrite existing entries on selected days* — control **enabled**, **off by
    default**. Off = append to the day; On = clear the day first.
  - Randomize only touches the **selected** days.
- User lockdown = hide nav/links AND redirect on direct URL access when the role
  is insufficient. User menu: Home, Calendar, Find Recipes, Recipes, User settings.

## Architecture touch points

- Persistence: new `MealPlanEntry` entity (UserId, Date, RecipeId) + EF config +
  DbSet + migration. User scoped by `IdentityUser.Id` (string). Recipe FK cascades.
- DTOs: `MealPlanEntryDto`, `AddMealPlanEntryDto`, `RandomizeMealPlanDto`; new
  `ApiRoutes.MealPlan`.
- Features (CQRS/MediatR): `MealPlan/Queries/GetMealPlan`,
  `Commands/AddMealPlanEntry`, `Commands/DeleteMealPlanEntry`,
  `Commands/RandomizeMealPlan`.
- API (FastEndpoints): `Endpoints/MealPlan/*` — policy `Admin,Moderator,User`,
  user id from the JWT `sub`/`NameIdentifier` claim.
- Web: `MealPlanService`, `Pages/Calendar/MealCalendar.razor`, user home split in
  `Pages/Home.razor`, `NavMenu` update, reusable `Components/RoleGuard.razor` on
  the admin/mod-only pages.

## Phases

1. Persistence — entity, configuration, DbSet, migration.
2. Contracts — DTOs + `ApiRoutes.MealPlan`.
3. Features — CQRS handlers (incl. randomize logic).
4. API — FastEndpoints endpoints.
5. Web plumbing — `MealPlanService` + DI registration.
6. Web UI — role home split, NavMenu, `RoleGuard` + guards, calendar page +
   randomize popup.
7. Build, verify wiring, docs, commit.

Each phase is committed on `feature/calendar` (never pushed — the user pushes).
