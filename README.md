# 🍽️ FoodHub

**A recipe and ingredient management system built with .NET 10 and Blazor Server**

FoodHub is a modern web application that helps you manage recipes, track ingredients, and generate shopping lists. Whether you're a home cook or managing a kitchen, FoodHub makes meal planning and ingredient management effortless.

FoodHub will become more than just a recipe app. It will evolve into a comprehensive food management system, integrating features like meal planning, inventory tracking, and nutritional analysis. The goal is to create a one-stop solution for all your culinary needs, making it easier to cook, shop, and eat healthily.

## ✨ Features

- 📝 **Recipe Management**: Create, edit, and organize your favorite recipes
- 🥘 **Ingredient Management**: Manage ingredients with quantities and measurement units
- 🛒 **Shopping List Generation**: Automatically generate shopping lists from selected recipes
- 📊 **Ingredient Aggregation**: Combine ingredients across multiple recipes to avoid duplicates
- 🔐 **Authentication & Roles**: JWT-based login with admin user/role management
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

The UI is a self-authored component library (`FoodCalc.Web/Components/UI/`) — buttons, cards, form fields, data table, tabs, toasts, modals — each with scoped CSS. Theming is token-driven:

- `wwwroot/css/tokens.css` — design tokens (colors, spacing, radius, shadows, type) with dark-mode overrides under `[data-bs-theme="dark"]`
- `wwwroot/css/theme.css` — maps the tokens onto Bootstrap 5.3 CSS variables (including per-component overrides) so plain Bootstrap markup picks up the theme
- `wwwroot/css/utilities.css` — small `fh-`prefixed layout/utility layer

Dark mode uses Bootstrap 5.3's `data-bs-theme`; an inline script in `App.razor` applies the persisted theme before first paint, so there is no flash of the wrong theme. No Bootstrap JavaScript is used — all interactive components are Blazor-state-driven.

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express)

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

## 🤝 Contributing

2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request


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
- [ ] Meal planning calendar
- [ ] Inventory management

---


# API results & the `Notify` helpers

How the frontend talks to the API and surfaces errors, and how the fluent
`ApiResult` helpers (`OnSuccess`, `OnFailure`, `Notify`, `OrDefault`) collapse the
repetitive success/error plumbing at call sites.

## The pieces

| Type | Job |
| --- | --- |
| `AuthenticatedHttpClientService` | The single choke-point for every API call. Attaches the bearer token, sends the request, **never throws**, and returns an `ApiResult`. On failure it reads the response body to recover the server's real message (ProblemDetails / FastEndpoints validation). |
| `ApiResult` / `ApiResult<T>` | The outcome of a call: `Success`, a user-ready `Error` string, a `StatusCode`, and (for `ApiResult<T>`) a `Data` payload. |
| `ApiResultExtensions` | Fluent helpers (`OnSuccess`, `OnFailure`, `Notify`, `OrDefault`) so components handle that outcome in one line. |
| `MessageService` | Synchronous toast relay. `ShowMessage(text, isError, timeInMs)` raises a toast; `ToastHost` renders and auto-dismisses it. |

The important part: **the error message is already parsed for you** by the HTTP
client. By the time a component sees an `ApiResult`, `Error` holds a clean,
user-ready string. Components never parse response bodies. See
[error-handling.md](error-handling.md) for how that message is recovered from the
server response.

## `ApiResult` in brief

```csharp
public class ApiResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }   // clean, user-ready message; null on success
    public int StatusCode { get; init; }
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }          // payload on success
}
```

Guarantees the helpers rely on:

- On **failure**, `Error` is always set (non-null).
- On **success** of an `ApiResult<T>`, `Data` is present.

## The helpers

All of them live in `ApiResultExtensions` and **return the same result**, so they
chain and can be guarded on (`if (!result.Notify(...).Success) ...`).

### `Then` / `OnSuccess` / `OnFailure` — run a side effect

```csharp
result.Then(() => ...);             // runs on success, no payload needed
result.OnSuccess(data => ...);      // runs on success, with the payload (ApiResult<T>)
result.OnFailure(error => ...);     // runs on failure, with the error message
```

`Then` and `OnSuccess` are the same guard — pick by whether you need the payload.
Both preserve the result type, so a `Then` on an `ApiResult<T>` can still be
followed by an `OnSuccess(data => ...)`.

> The action is a **lambda**, so it runs *later, only on success*. Passing a call
> directly — `Then(Navigation.NavigateTo("/login"))` — does not work: it returns
> `void` (can't be an argument) and would run immediately, regardless of outcome.
> Always wrap it: `Then(() => Navigation.NavigateTo("/login"))`.

### `Notify` — toast the outcome

```csharp
result.Notify(messages);                         // failure -> error toast; success -> silent
result.Notify(messages, success: "Saved!");      // failure -> error toast; success -> success toast
result.Notify(messages, errorTimeInMs: 0);       // sticky error toast (0 = until dismissed)
```

`Notify` shows **at most one** toast:

- **Failure** → shows `Error` as an error toast.
- **Success** → shows the `success` message as a success toast, or nothing if you
  didn't pass one.

Signature:

```csharp
ApiResult Notify(this ApiResult result, MessageService messages,
    string? success = null,
    int successTimeInMs = MessageService.DefaultDisplayTimeInMs,   // 12000
    int errorTimeInMs   = MessageService.DefaultDisplayTimeInMs)
```

### `OrDefault` — payload or a fallback

```csharp
T value = result.OrDefault(fallback);   // Data on success, fallback on failure
```

## Recipes (the three common shapes)

### 1. Load into state, toast on failure

Await inline, `Notify` the failure, then take the data or a fresh empty value.

```csharp
// Home.razor
private async Task LoadRecipesAsync()
{
    pagedResult = (await recipeService.GetPagedRecipesAsync(currentPage, pageSize, searchTerm))
        .Notify(MessageService, errorTimeInMs: 0)   // sticky error if the load fails
        .OrDefault(new());                           // otherwise the paged data
}
```

Silent fallback (no toast at all) is just `OrDefault`:

```csharp
// RecipeBlackList.razor
AllRecipes = (await RecipeService.GetAllRecipesAsync(false)).OrDefault(new());
```

### 2. Mutate + toast both outcomes

`Then` does the local state change (no payload needed); `Notify` handles the
success **and** error toast.

```csharp
// UserRoles.razor
private async Task RemoveRole(string role)
{
    (await AdminService.RemoveUserRoleAsync(email, role))
        .Then(() => roles.Remove(role))
        .Notify(MessageService, success: WebConstants.Messages.Roles.Removed);
}
```

Multi-line success work reads the same way:

```csharp
// UserRoles.razor
(await AdminService.UpdateUserRolesAsync(email, selectedRole))
    .Then(() =>
    {
        roles.Add(selectedRole);
        selectedRole = string.Empty;
    })
    .Notify(MessageService, success: WebConstants.Messages.Roles.Added);
```

### 3. Guard and continue

When there's more work after the call that shouldn't be nested in a lambda,
`Notify` returns the result so you can guard on `.Success`. The failure toast
fires as a side effect; on success you fall through.

```csharp
// EditRecipe.razor
private async Task AddOrUpdateIngredient()
{
    recipeIngredient.RecipeId = RecipeId;
    recipeIngredient.Amount = amountInput ?? 0;

    if (!(await RecipeService.AddIngredient(recipeIngredient)).Notify(MessageService).Success)
        return;

    MessageService.ShowMessage(WebConstants.Messages.Ingredient.AddedOrUpdated, false);
    ClearForm();
    await RefreshRecipeAsync();
}
```

```csharp
// UserList.razor
public async Task ToggleUserAsync(string email, bool enabled)
{
    if (!(await AdminService.ToggleUserAsync(email, enabled)).Notify(MessageService).Success)
        return;

    var user = pagedResult?.Items.FirstOrDefault(u => u.Email == email);
    if (user != null)
        user.Enabled = enabled;
}
```

Simple "reload on success" is the same guard inline:

```csharp
// Home.razor
if ((await recipeService.DeleteRecipe(recipeId)).Notify(MessageService).Success)
    await LoadRecipesAsync();
```

## Before / after

```csharp
// before
var result = await AdminService.RemoveUserRoleAsync(email, role);
if (result.Success)
{
    roles.Remove(role);
    await MessageService.ShowMessageAsync(WebConstants.Messages.Roles.Removed, false);
}
else
{
    await MessageService.ShowMessageAsync(result.Error!, true);
}

// after
(await AdminService.RemoveUserRoleAsync(email, role))
    .Then(() => roles.Remove(role))
    .Notify(MessageService, success: WebConstants.Messages.Roles.Removed);
```

Navigate on success, toast the error otherwise:

```csharp
// Register.razor
(await LoginService.RegisterAsync(registerModel))
    .Notify(MessageService, success: WebConstants.Messages.Auth.RegisterSuccess)
    .Then(() => Navigation.NavigateTo("/login"));
```

## When *not* to use them

- **Direct messages** that aren't the outcome of an `ApiResult` — validation
  hints, exception messages in a `catch` — call `MessageService.ShowMessage(...)`
  directly:

  ```csharp
  // DataImportExport.razor
  catch (Exception ex)
  {
      messageService.ShowMessage($"Import failed: {ex.Message}", true);
  }
  ```

- **Branching that needs the payload before deciding what to toast** (e.g. login
  distinguishes "request failed" from "no token in response") — plain
  `if (result.Success)` is clearer than forcing it through a helper.

## Notes

- `Notify` shows one toast per call. If both a success message and different
  timings for success vs. error matter, use `successTimeInMs` / `errorTimeInMs`.
- Helpers are synchronous — `MessageService.ShowMessage` is fire-and-forget, so
  there's nothing to `await`. Await the API call, then chain.
- `OrDefault` returns `Data` on success (guaranteed non-null) or your fallback;
  it never toasts. Combine with `Notify` when you also want the error surfaced.

## Related

- [error-handling.md](error-handling.md) — how `AuthenticatedHttpClientService`
  turns a failed response into the clean `Error` string these helpers consume.
