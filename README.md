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
- **Database**: SQL Server with Entity Framework Core (Code-First migrations)
- **Orchestration**: .NET Aspire AppHost
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
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
  or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express)

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

3. **Update connection string**
   Update the connection string in `FoodCalc.Api/appsettings.json` to point to your SQL Server instance.

4. **Run database migrations**
   ```bash
   dotnet ef database update --project FoodHub.Persistence --startup-project FoodCalc.Api
   ```

5. **Start the application**
   ```bash
   dotnet run --project FoodHub.AppHost
   ```

6. **Access the application**
	- Web Application: The URL will be displayed in the console (typically https://localhost:7xxx)
	- API Documentation: Available at the API's `/swagger` endpoint in development

## 🐳 Run with Docker

A self-contained stack (SQL Server **db** + **api** + **web**, no Aspire AppHost) is defined in [
`docker-compose.yml`](docker-compose.yml). Everything runs over HTTP on the internal Docker network (port `8080`); only
the web app needs to be reached from a browser.

```bash
# Build images and start the stack in the background
docker compose up -d --build
```

- **Web app**: http://localhost:5002
- **API**: http://localhost:5001 (optional; the Blazor server calls it internally)
- **SQL Server**: `localhost:1433`

Startup order is handled automatically: `db` (waited on via healthcheck) → `api` →
`web`. The API connects as `sa` and runs EF Core migrations on boot, which create the
`FoodCalc` database and the entire schema — no separate init script is required.

### Default accounts

The `SeedDefaultUsers` migration seeds the `Admin`/`Moderator`/`User` roles and two sign-in-ready accounts so a fresh
database has a working login:

| Account | Email                 | Password    | Roles                  |
|---------|-----------------------|-------------|------------------------|
| Admin   | `admin@foodhub.local` | `Admin123!` | Admin, Moderator, User |
| User    | `user@foodhub.local`  | `User123!`  | User                   |

> 🔒 These are development defaults committed to the repo — **change the passwords
> after the first login** (or delete/replace the accounts) before exposing the app.

### Data persistence

The database is stored in the named volume `mssql-data`, and DataProtection keys (auth/antiforgery) in
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
