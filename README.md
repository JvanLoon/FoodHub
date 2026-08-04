# 🍽️ FoodHub

**A recipe and ingredient management system built with .NET 10 and Blazor Server**

FoodHub is a modern web application that helps you manage recipes, track ingredients, and generate shopping lists.
Whether you're a home cook or managing a kitchen, FoodHub makes meal planning and ingredient management effortless.

FoodHub will become more than just a recipe app. It will evolve into a comprehensive food management system, integrating
features like meal planning, inventory tracking, and nutritional analysis. The goal is to create a one-stop solution for
all your culinary needs, making it easier to cook, shop, and eat healthily.

## ✨ Features

- 📝 **Recipe Management**: Create, edit, and organize your favorite recipes
- 📅 **Meal Calendar**: Plan recipes per day in side-by-side week and month views, saved per user. Select two or more
  days and **Randomize** to auto-fill them with random recipes (optionally biased by ingredients you want)
- 🥘 **Ingredient Management**: Manage ingredients with quantities and measurement units
- 🛒 **Shopping List Generation**: Automatically generate shopping lists from selected recipes
- 📊 **Ingredient Aggregation**: Combine ingredients across multiple recipes to avoid duplicates
- 🔐 **Authentication & Roles**: JWT-based login with admin user/role management. Regular users get a focused experience
  (Home, Calendar, Recipes, Find recipes, User settings); admin-only pages redirect insufficient roles to Home, and
  admins land on the stats dashboard
- 🌗 **Light & Dark Mode**: Persisted theme toggle, defaults to your system preference
- 🌐 **Modern Web Interface**: Responsive Blazor Server UI with a custom component library
- 🔄 **API**: Fast, minimal HTTP API built on FastEndpoints

## 🛠️ Technology Stack

- **Backend**: .NET 10, [FastEndpoints](https://fast-endpoints.com/) (REPR pattern), FluentValidation
- **Frontend**: Blazor Server (interactive server rendering), Bootstrap 5.3, Bootstrap Icons
- **Database**: PostgreSQL with Entity Framework Core (Code-First migrations, Npgsql provider)
- **Orchestration**: .NET Aspire AppHost — provisions PostgreSQL + pgAdmin as Docker containers
- **Mapping**: Hand-written mapping (no AutoMapper)
- **Dependency Injection**: Built-in .NET DI container

### Frontend design

The UI is a self-authored component library (`FoodCalc.Web/Components/UI/`) — buttons, cards, form fields, data table,
tabs, toasts, modals — each with scoped CSS. Theming is token-driven:

- `wwwroot/css/tokens.css` — design tokens (colors, spacing, radius, shadows, type) with dark-mode overrides under
  `[data-bs-theme="dark"]`
- `wwwroot/css/theme.css` — maps the tokens onto Bootstrap 5.3 CSS variables (including per-component overrides) so
  plain Bootstrap markup picks up the theme
- `wwwroot/css/utilities.css` — small `fh-`prefixed layout/utility layer

Dark mode uses Bootstrap 5.3's `data-bs-theme`; an inline script in `App.razor` applies the persisted theme before first
paint, so there is no flash of the wrong theme. No Bootstrap JavaScript is used — all interactive components are
Blazor-state-driven.

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — the Aspire AppHost runs PostgreSQL and pgAdmin as
  containers, so **no local PostgreSQL install is needed**

### Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/JvanLoon/FoodHub.git
   cd FoodHub
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Start the application**
   ```bash
   dotnet run --project FoodHub.AppHost
   ```
   Aspire starts a **PostgreSQL** container (recreated on each run; data persists in the `foodhub-pgdata`
   volume) and a **pgAdmin** container, then runs the API and Web projects pointed at that database. The API applies EF
   Core migrations on boot, so the schema is created automatically — no manual migration step and no connection string
   to edit.

4. **Access the application**
    - Web Application, API, and **pgAdmin** links are all listed in the Aspire dashboard that opens on start
    - API Documentation: Available at the API's `/swagger` endpoint in development

> To run without Aspire (plain `dotnet run` on the API), point `ConnectionStrings:DefaultConnection` in
> `FoodCalc.Api/appsettings.Development.json` at a reachable PostgreSQL instance — e.g. start just the database
> with `docker compose up -d db pgadmin`.

## 🐳 Run with Docker

A self-contained stack (PostgreSQL **db** + **pgAdmin** + **api** + **web**, no Aspire AppHost) is defined in [
`docker-compose.yml`](docker-compose.yml). The app services run over HTTP on the internal Docker network (port `8080`);
only the web app and pgAdmin need to be reached from a browser.

```bash
# Build images and start the stack in the background
docker compose up -d --build
```

- **Web app**: http://localhost:5002
- **API**: http://localhost:5001 (optional; the Blazor server calls it internally)
- **pgAdmin**: http://localhost:5050 (login `admin@foodhub.local` / `admin`)
- **PostgreSQL**: `localhost:5432` (user `foodhub`, database `FoodCalc`)

Startup order is handled automatically: `db` (waited on via healthcheck) → `api` →
`web`. The API runs EF Core migrations on boot, which create the entire schema — no separate init script is required.

To browse tables in pgAdmin, add a server pointing at host `db`, port `5432`, user `foodhub`. Its **restore/import**
feature is the easiest way to reset the data.

### Getting a first account

**Nothing is seeded.** There are no default accounts, and the two that used to be committed here
(`admin@foodhub.local` / `user@foodhub.local`) are deleted by the
`RemoveSeededIdentityData` migration. Assume their published passwords are burned.

The `Admin`/`Moderator`/`User` roles are created at runtime on every boot. To get the first account, set these two
variables **once** and start the API:

```
Bootstrap__AdminEmail=you@example.com
Bootstrap__AdminPassword=<generated>
```

The account is created only while the users table is completely empty, so it cannot overwrite anything or run twice.
Sign in, change the password, then clear both variables. See [deployment.md](deployment.md) for the full story.

### Enabling an account — use the UI, not SQL

New registrations arrive disabled. Enabling one from the admin **Gebruikers** tab does *two* things: it clears
the lockout, **and** it grants the `User` role.

Flipping `EmailConfirmed` / `LockoutEnabled` directly in the database only does the first. You get a **floating
account**: it signs in fine and looks completely normal, but it holds no role at all, so every role-gated
endpoint answers `403` for it. The visible symptom is presence — the heartbeat and sign-out endpoints are
role-gated, so the account never shows a green dot in the user list and never gets a "laatst actief" time,
however much the person is actually using the app.

> ⚠️ Nothing warns you about this. The account works, so it looks fine until you notice it is never online.

To repair one, either add the `User` role from **Gebruikersrollen**, or disable and re-enable the account from
the **Gebruikers** tab — the enable path grants the role whenever it is missing.

### Data persistence

The database is stored in the named volume `pgdata`, and DataProtection keys (auth/antiforgery) in
`dataprotection-keys`. Both survive container recreation:

```bash
docker compose down           # stop & remove containers — DATA IS KEPT
docker compose up -d          # recompose — recipes/users are still there
```

> ⚠️ `docker compose down -v` deletes the volumes and wipes the database. Don't use `-v`
> unless you intend to start from scratch.

## 🔧 Development

### Database Migrations

```bash
# Add a new migration
dotnet ef migrations add <MigrationName> --project FoodHub.Persistence --startup-project FoodCalc.Api

# Apply migrations
dotnet ef database update --project FoodHub.Persistence --startup-project FoodCalc.Api

# Remove last migration
dotnet ef migrations remove --project FoodHub.Persistence --startup-project FoodCalc.Api

# Revert to specific migration
dotnet ef database update --project FoodHub.Persistence --startup-project FoodCalc.Api --migration <MigrationName>
```

OR

```bash
# Add a new migration
Add-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api -Name <MigrationName>

# Apply migrations
Update-Database -Context FoodHubDbContext -Project FoodHub.Persistence -StartupProject FoodCalc.Api

# Remove last migration
Remove-Migration -Project FoodHub.Persistence -StartupProject FoodCalc.Api

# Revert to specific migration
Update-Database -Context FoodHubDbContext -Project FoodHub.Persistence -StartupProject FoodCalc.Api -Migration <MigrationName>
```

### API Documentation

When running the API in development, Swagger (NSwag via FastEndpoints) is available at `/swagger`.

### Error handling

Endpoints return **every** error a request produced, not just the first, as RFC9457 ProblemDetails.
`FoodCalc.Api/Common/ErrorResultExtensions.cs` provides
`SendErrorsAsync`; the Blazor client parses the response into `ApiResult.Errors`
and toasts each message.

- [FoodCalc.Web/Components/Services/README.md](FoodCalc.Web/Components/Services/README.md)
  — the `ApiResult` helpers (`OnSuccess`, `OnFailure`, `OrDefault`) components use.
- [FoodCalc.Web/Components/Services/error-handling.md](FoodCalc.Web/Components/Services/error-handling.md)
  — how a failed response becomes those clean messages.

## 🤝 Contributing

1. Create a feature branch (`git checkout -b feature/amazing-feature`)
2. Commit your changes (`git commit -m 'Add some amazing feature'`)
3. Push to the branch (`git push origin feature/amazing-feature`)
4. Open a Pull Request

## 🆘 Support

If you encounter any issues or have questions:

1. Check the [Issues](https://github.com/JvanLoon/FoodHub/issues) page
2. Create a new issue if your problem isn't already reported
3. Provide detailed information about your environment and the issue

## 🔮 Roadmap

- [x] User authentication and authorization
- [x] Dark mode and revamped UI (design tokens + custom component library)
- [ ] Recipe categories and tags
- [ ] Nutritional information tracking
- [ ] Recipe sharing and community features
- [ ] Mobile app (Xamarin/MAUI)
- [ ] Recipe import from popular cooking websites
- [x] Meal planning calendar (week/month views, per-day recipes, randomize)
- [ ] Inventory management
