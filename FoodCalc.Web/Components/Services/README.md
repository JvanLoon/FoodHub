# API results & the `ApiResult` helpers

How the frontend talks to the API and surfaces errors, and how the fluent
`ApiResult` helpers (`OnSuccess`, `OnFailure`, `OrDefault`) collapse the
repetitive success/error plumbing at call sites.

## The pieces

| Type | Job |
| --- | --- |
| `AuthenticatedHttpClientService` | The single choke-point for every API call. Attaches the bearer token, sends the request, **never throws**, and returns an `ApiResult`. On failure it reads the response body to recover every message the server reported. |
| `ApiResult` / `ApiResult<T>` | The outcome of a call: `Success`, a user-ready `Errors` list, a `StatusCode`, and (for `ApiResult<T>`) a `Data` payload. |
| `ApiResultExtensions` | Fluent helpers (`OnSuccess`, `OnFailure`, `OrDefault`) so components handle that outcome in one line. |
| `MessageService` | Synchronous toast relay. `ShowMessage(text, isError, timeInMs)` raises a toast; `ToastHost` renders and auto-dismisses it. |

Two things the helpers depend on:

- **The error messages are already parsed for you** by the HTTP client. By the
  time a component sees an `ApiResult`, `Errors` holds clean, user-ready strings.
  Components never parse response bodies. See [error-handling.md](error-handling.md)
  for how those messages are recovered from the server response.
- **The `MessageService` is already attached.** `AuthenticatedHttpClientService`
  stamps it onto every result, so you never pass it to a helper.

## `ApiResult` in brief

```csharp
public class ApiResult
{
    public bool Success { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];  // user-ready messages; empty on success
    public int StatusCode { get; init; }                      // 0 for transport/exception failures
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }          // payload on success
}
```

Guarantees the helpers rely on:

- On **failure**, `Errors` holds at least one message.
- On **success** of an `ApiResult<T>`, `Data` is present.

`Errors` is a list because the API reports *every* problem it found, not just the
first. Most responses still carry exactly one.

## The helpers

All of them live in `ApiResultExtensions`. They are extension methods on
**`Task<ApiResult>`**, not on `ApiResult` — so you chain directly onto the service
call and `await` the whole chain, rather than awaiting first:

```csharp
await RecipeService.DeleteRecipe(id).OnSuccess(...).OnFailure();
```

Each helper returns the same result, so the order above is the normal shape.

### `OnSuccess` — side effect + optional success toast

```csharp
.OnSuccess(() => roles.Remove(role))                      // no payload needed
.OnSuccess(data => ingredients = data)                    // with the payload (ApiResult<T>)
.OnSuccess(() => roles.Remove(role), "Role removed")      // + success toast
.OnSuccess(message: "Import succeeded")                   // toast only, no action
```

> The action is a **lambda**, so it runs *later, only on success*. Passing a call
> directly — `OnSuccess(Navigation.NavigateTo("/login"))` — does not work: it
> returns `void` (can't be an argument) and would run immediately, regardless of
> outcome. Always wrap it: `OnSuccess(() => Navigation.NavigateTo("/login"))`.

The generic overload is constrained to `where T : class`.

### `OnFailure` — toast the errors

```csharp
.OnFailure()                    // shows one error toast per message
.OnFailure(errors => ...)       // ...and hands you the IReadOnlyList<string>
```

**`OnFailure()` is what surfaces errors.** `OnSuccess` alone toasts nothing on
failure, so a chain that omits `OnFailure()` fails silently. The only exception is
`OrDefault`, which toasts on its own.

One toast per message — usually one, but an endpoint may return several and none
of them are dropped.

### `OrDefault` — payload or a fallback

```csharp
T value = await result.OrDefault(fallback);   // Data on success, fallback on failure
```

Toasts the errors as a side effect, so it does **not** need a trailing
`OnFailure()`. Unlike `OnSuccess`, it has no `class` constraint.

### Toast timing

Fixed at `MessageService.DefaultDisplayTimeInMs` (12 000 ms). There is no
per-call override; for anything else call `MessageService.ShowMessage(...)`
directly.

## Recipes (the common shapes)

### 1. Load into state, toast on failure

`OrDefault` takes the data or a fresh empty value, and toasts the failure itself.

```csharp
// RecipeList.razor
pagedResult = await recipeService.GetPagedRecipesAsync(currentPage, pageSize, searchTerm)
    .OrDefault(new PagedResultDto<RecipeDto>());
```

When the state assignment needs more than one line, use `OnSuccess` + `OnFailure`:

```csharp
// EditRecipe.razor
await IngredientService.GetAllIngredientsAsync()
    .OnSuccess(data =>
    {
        ingredients = data;
        ClearForm();
    })
    .OnFailure();
```

### 2. Mutate + toast both outcomes

`OnSuccess` does the local state change and the success toast; `OnFailure` covers
the error.

```csharp
// UserRoles.razor
await AdminService.RemoveUserRoleAsync(email, role)
    .OnSuccess(() => roles.Remove(role), WebConstants.Messages.Roles.Removed)
    .OnFailure();
```

Multi-line success work reads the same way:

```csharp
// UserRoles.razor
await AdminService.UpdateUserRolesAsync(email, selectedRole)
    .OnSuccess(() =>
    {
        roles.Add(selectedRole);
        selectedRole = string.Empty;
    },
    WebConstants.Messages.Roles.Added)
    .OnFailure();
```

### 3. Navigate on success

```csharp
// Register.razor
await LoginService.RegisterAsync(registerModel)
    .OnSuccess(() => Navigation.NavigateTo("/login"), WebConstants.Messages.Auth.RegisterSuccess)
    .OnFailure();
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
  distinguishing "request failed" from "no token in response") — a plain
  `if (result.Success)` is clearer than forcing it through a helper.

## Notes

- Helpers are synchronous internally — `MessageService.ShowMessage` is
  fire-and-forget, so there's nothing to `await` inside a chain. Await the chain
  itself.
- `OrDefault` returns `Data` on success (guaranteed non-null) or your fallback.

## Related

- [error-handling.md](error-handling.md) — how `AuthenticatedHttpClientService`
  turns a failed response into the clean `Errors` list these helpers consume.
- [../../../README.md](../../../README.md) — project overview and setup.
