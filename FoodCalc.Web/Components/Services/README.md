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
user-ready string. Components never parse response bodies.

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

### `OnSuccess` / `OnFailure` — run a side effect

```csharp
result.OnSuccess(() => ...);        // ApiResult      — runs when Success
result.OnSuccess(data => ...);      // ApiResult<T>   — runs with the payload
result.OnFailure(error => ...);     // runs with the error message when it failed
```

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

`OnSuccess` does the local state change; `Notify` handles the success **and**
error toast.

```csharp
// UserRoles.razor
private async Task RemoveRole(string role)
{
    (await AdminService.RemoveUserRoleAsync(email, role))
        .OnSuccess(() => roles.Remove(role))
        .Notify(MessageService, success: WebConstants.Messages.Roles.Removed);
}
```

Multi-line success work reads the same way:

```csharp
// UserRoles.razor
(await AdminService.UpdateUserRolesAsync(email, selectedRole))
    .OnSuccess(() =>
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
    .OnSuccess(() => roles.Remove(role))
    .Notify(MessageService, success: WebConstants.Messages.Roles.Removed);
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
