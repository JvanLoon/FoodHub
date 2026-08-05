# Plan: move the auth token out of localStorage into an httpOnly cookie

Status: **not started**. This is a design + step plan, written to be handed to a fresh session.
Nothing in it has been implemented.

Branch to start from: `feature/auth-hardening` (or wherever it has landed by then).

---

## 1. Why

The JWT currently lives in browser `localStorage`, so any XSS can read it and walk away with a
valid 12-hour credential. Moving it to an httpOnly cookie makes it unreadable from script.

Be clear about what this does and does not buy:

* **Buys:** the token can no longer be *stolen*.
* **Does not buy:** XSS can still *act as* the user, because a cookie is attached automatically.
* **Costs:** CSRF becomes relevant, where localStorage was immune to it. `UseAntiforgery()` is
  already in the pipeline but stops being incidental and becomes load-bearing.

Note the value is lower than it was a week ago: tokens are now revocable via the security stamp
(`FoodCalc.Api/Common/SecurityStampCheck.cs`), so a stolen token can be killed instantly rather
than being good for twelve hours. Weigh that before committing to the work.

---

## 2. The insight that makes this tractable

**The browser never talks to the API.** `AuthenticatedHttpClientService` runs in the Blazor
**Server** process; in production `API__BaseAddress=http://api:8080`, an internal address, and the
`api` service publishes no ports at all.

So the token is currently making a pointless round trip to the browser purely so the *server* can
read it back via JS interop. After this change the JWT never leaves the server, and the browser
holds only an opaque encrypted cookie.

**The API needs no changes whatsoever.** It keeps validating `Authorization: Bearer` exactly as it
does today. This is entirely a `FoodCalc.Web` change.

---

## 3. Three constraints that shape the design

1. **Cookies cannot be written over the SignalR circuit.** `HttpContext.SignInAsync` /
   `SignOutAsync` only work during a real HTTP request. Login and logout therefore cannot stay as
   `@onclick` / `OnValidSubmit` handlers on interactive components — they must become HTTP
   endpoints or static-SSR form posts. This is the single biggest source of friction and it bites
   in three places: login, logout, and the 401 handler.

2. **There is no `HttpContext` inside a circuit.** The JWT has to be captured once during the
   initial HTTP render into a scoped service, then read from there for the life of the circuit.

3. **`prerender: false` does not remove the HTTP render.** `App.razor` is still rendered
   statically over HTTP to deliver the document, so there *is* a request in which to read the
   cookie. Only the interactive components skip prerendering.

---

## 4. Design decisions (make these deliberately)

**Where the JWT is stored server-side.** Recommend inside the auth cookie's
`AuthenticationProperties` (`props.StoreTokens` / `props.Items`). It is encrypted by Data
Protection, needs no server-side state, and survives restarts and scale-out. DP keys are already
persisted to the `dataprotection-keys` volume, so this works across container recreation.
Watch the 4KB cookie limit — the current JWT with three role claims is well under it, but check.

The alternative (server memory keyed by a session id) adds state and dies on restart. Don't.

**Cookie lifetime.** Align `ExpiresUtc` with the JWT's `exp` (12h). If the cookie outlives the
JWT the UI looks signed in while every API call 401s.

**Keep the JWT.** Do not be tempted to drop it and have the web app call the API with a service
credential — that would lose per-user authorisation at the API, which is where it is actually
enforced.

---

## 5. Current state — what you are replacing

Auth today is entirely client-side state read out of the token.

| File | What it does |
|---|---|
| `FoodCalc.Web/Services/Auth/AuthTokenService.cs` | **The only** auth toucher of localStorage. Reads/writes key `Authorization` (`WebConstants.Storage.AuthToken`), parses claims with `ReadJwtToken` |
| `FoodCalc.Web/Services/Auth/AuthStateService.cs` | `IsLoggedInAsync`, `GetRolesAsync`, `IsAdminAsync`, `IsInAnyRoleAsync`, `GetUserIdAsync`, `GetEmailAsync`, `CanEditAnyContentAsync`, `CanEditContentAsync`, `SignInAsync`, `SignOutAsync`, `DiscardTokenAsync`, `OnAuthStateChanged` event |
| `FoodCalc.Web/Services/Auth/AuthenticatedHttpClientService.cs` | Attaches the bearer; `AbandonSessionAsync()` handles a 401 that carried a token |
| `FoodCalc.Web/Services/Auth/PresenceService.cs` | 60s heartbeat over the same client; `SignalOfflineAsync()` must run **while the credential is still usable** |

### Every call site to migrate (14 across 11 files)

```
Components/RoleGuard.razor:27        GetRolesAsync
Layout/MainLayout.razor:97,100,102,107,119,125,130   OnAuthStateChanged, IsLoggedIn, SignOut, GetEmail
Layout/NavMenu.razor:58,72,73,74,81  OnAuthStateChanged, IsLoggedIn, IsAdmin, IsInAnyRole
Pages/Admin/Admin.razor:57           IsAdminAsync
Pages/Admin/Home.razor:157           IsAdminAsync
Pages/Admin/UserList.razor:128       GetUserIdAsync
Pages/Auth/Login.razor:45,56,70      IsLoggedIn, DiscardToken, SignIn
Pages/Recipes/EditRecipe.razor:242   CanEditContentAsync
Pages/Recipes/RecipeList.razor:141,142  CanEditAnyContent, GetUserIdAsync
Pages/User/UserSettings.razor:36     AuthTokenService.GetEmailAsync
```

`Pages/Recipes/ShoppingList.razor:6` injects `AuthStateService` and never uses it — delete that
line while you are in there.

### What replaces what

| Today | After |
|---|---|
| `GetTokenAsync()` | scoped `TokenProvider`, seeded from the cookie during the HTTP render |
| `IsLoggedInAsync()` | `AuthenticationState` / `<AuthorizeView>` |
| `GetRolesAsync()`, `IsAdminAsync()`, `IsInAnyRoleAsync()` | `ClaimsPrincipal.IsInRole(...)` |
| `GetUserIdAsync()`, `GetEmailAsync()` | claims on the principal |
| `IsTokenExpiredAsync()` | cookie expiry, handled by the auth middleware |
| `OnAuthStateChanged` event | `AuthenticationStateProvider` change notification |
| `RoleGuard` | `[Authorize(Roles = "...")]` + `AuthorizeRouteView`, or keep the component reading `AuthenticationState` |

---

## 6. Steps

Do these in order. Each should build; commit per step.

**Step 1 — cookie authentication scaffolding.**
In `FoodCalc.Web/Program.cs` add `AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)`
+ `AddCookie` (HttpOnly, `SecurePolicy=Always`, `SameSite=Strict`, `LoginPath=/login`),
`AddAuthorization()`, `AddCascadingAuthenticationState()`, and restore `app.UseAuthentication()`
/ `app.UseAuthorization()` — they were removed in commit `95118a6` because nothing consumed them,
and this is what makes them earn their place again.
Add `AddHttpContextAccessor()`.

**Step 2 — login/logout as HTTP endpoints.**
`MapPost("/auth/login")`: call the API's `api/authentication/login`, take the JWT, build a
`ClaimsPrincipal` from its claims (sub, email, roles), `SignInAsync` with the JWT in
`AuthenticationProperties`, redirect to `/`.
`MapPost("/auth/logout")`: POST `api/authentication/signout` first (presence goes offline while
the JWT still works), then `SignOutAsync`, redirect to `/login`.
Both must be antiforgery-protected.

**Step 3 — convert `Login.razor` to a static SSR form.**
It currently uses an interactive `EditForm` with `OnValidSubmit`, which cannot set a cookie. Use
`<form method="post" action="/auth/login">` with `[SupplyParameterFromForm]` and an
`AntiforgeryToken`. Keep the existing "already signed in → redirect" behaviour, but drive it off
`AuthenticationState` rather than a token read.

**Step 4 — `TokenProvider`.**
Scoped service holding the JWT for the circuit, seeded during the static render from
`HttpContext.GetTokenAsync(...)`. Point `AuthenticatedHttpClientService.AttachTokenAsync` at it
instead of `AuthTokenService`.

**Step 5 — migrate the 14 call sites** in the table above to `AuthenticationState` / claims.
Replace `RoleGuard` or re-point it. `MainLayout` and `NavMenu` lose their
`OnAuthStateChanged` subscriptions in favour of `<AuthorizeView>` / cascading state.

**Step 6 — rework `AbandonSessionAsync`.**
It cannot clear a cookie from inside the circuit. Change it to `NavigateTo("/auth/logout", forceLoad: true)`
so the sign-out happens in a real request. Keep the existing "only when a token was attached"
guard — a wrong password on the login form is also a 401 and must not trigger a logout.

**Step 7 — delete the dead code.**
`AuthTokenService` (or reduce it to claim parsing if anything still needs it),
`WebConstants.Storage.AuthToken`, the unused `ShoppingList` injection.
**Keep the `Blazored.LocalStorage` package** — `RecipeList.razor`, `FindByIngredients.razor` and
`ShoppingList.razor` use it for non-auth state, and `App.razor` uses raw localStorage for the
theme.

---

## 7. Verification

Follow the project's usual rule: a throwaway `postgres:17` container, never the live compose stack.

Must all pass:

1. `document.cookie` in the browser console does **not** reveal the auth cookie (proves HttpOnly).
2. `localStorage` holds no token after login — only `foodhub-theme` and the recipe/shopping keys.
3. Login → lands on `/`; wrong password → stays on the form with the error, no redirect loop.
4. Logout → cookie cleared, back to `/login`, and the user shows offline in the admin list.
5. Role gating: a non-admin cannot reach `/admin`; admin-only nav is hidden.
6. Revocation: disable the account from another client → next request bounces to `/login`.
7. Presence still works — heartbeat keeps the green dot alive on the admin users tab.
8. Restart the web container → still signed in (proves DP-key persistence covers the cookie).

---

## 8. Do not change

* **The API.** It keeps bearer-token validation, the security-stamp check, lockout, all of it.
* **The 12h token lifetime** — deliberate, decided.
* **`ValidateAudience = false`** in `FoodCalc.Api/Program.cs`. It defaults to `true`; deleting the
  line rejects every token with `IDX10208`.
* **`SecurityAlgorithms.HmacSha256`** in `LoginEndpoint` — `ValidAlgorithms` is pinned to `HS256`
  and the `HmacSha256Signature` constant emits a different `alg` header that will be refused.

---

## 9. Expect a forced logout

Everyone signs in again when this deploys — existing localStorage tokens stop being consulted.
Same as the security-stamp change. Worth deploying the two together if the stamp change has not
shipped yet.
