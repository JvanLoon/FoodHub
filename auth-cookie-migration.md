# Plan: move the auth token out of localStorage into an httpOnly cookie

Status: **done**, on `feature/auth-hardening` — `26384d7` (the migration) and `d721f35` (the
SameSite fallout). Verified against a throwaway `postgres:17` stack; all eight checks in §7 pass.
Kept as the record of why it is shaped the way it is.

Four things came out differently from the plan below. They are marked **[changed]** where they
apply, and the reasons are worth reading before touching any of this again:

1. **The JWT is a claim on the principal, not `AuthenticationProperties`.** §4 recommended
   properties. Reading those back needs an `HttpContext`, and a circuit has none — and the HTTP
   render and the circuit are *separate DI scopes*, so the "seed a scoped `TokenProvider` during
   the static render" of step 4 could never have worked either; the circuit's copy would always
   have been empty. A claim arrives in the circuit on the principal itself.
2. **There is no `/auth/login` endpoint.** The login page is statically rendered instead and signs
   in directly, so the form, its validation and its error rendering stay in one Razor file.
3. **`AuthStateService` survives**, reimplemented over claims, rather than every call site moving
   to `AuthenticationState` by hand. The "is this mine?" rules stay in one place and the diff
   stays small. `AuthTokenService` is gone as planned.
4. **The redirect to the login form is middleware, not `MainLayout`** — and `SameSite=Strict`
   needed a companion Lax cookie to stay usable. See §10, which the plan did not anticipate at all.

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

**Where the JWT is stored server-side.** **[changed]** The plan recommended
`AuthenticationProperties` (`props.StoreTokens` / `props.Items`). It went in as a private claim on
the `ClaimsPrincipal` instead — `AuthCookie.TokenClaim` — for the reason in the header: properties
can only be read through an `HttpContext`, and the circuit has none. Both end up in the same
Data-Protection-encrypted cookie, so nothing is given away by preferring the one that can be read
where it is needed.

Measured: the whole auth cookie is **1563 bytes** with three role claims, comfortably under the
4093-byte point where `ChunkingCookieManager` would start splitting it. DP keys are persisted to
the `dataprotection-keys` volume, so it survives container recreation — §7 check 8 proves it.

The alternative (server memory keyed by a session id) adds state and dies on restart. Don't.

**Cookie lifetime.** Align `ExpiresUtc` with the JWT's `exp` (12h). If the cookie outlives the
JWT the UI looks signed in while every API call 401s.

**Keep the JWT.** Do not be tempted to drop it and have the web app call the API with a service
credential — that would lose per-user authorisation at the API, which is where it is actually
enforced.

---

## 5. Current state — what you are replacing

*Historical: this describes the code before the change. Line numbers are stale.*

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

**Step 2 — logout as an HTTP endpoint.** **[changed]** There is no `/auth/login` endpoint; step 3
absorbed it. `MapGet("/auth/logout")` signs out and redirects to `/login`. A GET, not a POST, so
the circuit can simply navigate to it — safe precisely because the cookie is `SameSite=Strict`, so
a forged cross-site navigation there arrives with no session to end. The offline ping is the
caller's job and happens before the trip, while the JWT is still usable; `AbandonSessionAsync`
skips it, since a token that has just been refused could only 401 again.

**Step 3 — convert `Login.razor` to a static SSR form.** **[changed]** Done, but the page signs in
itself rather than posting elsewhere: `@attribute [ExcludeFromInteractiveRouting]` plus a
render-mode gate in `App.razor` (`HttpContext.AcceptsInteractiveRouting()`) means the request is
rendered statically and the page has a real `HttpContext`. Ordinary `EditForm` +
`[SupplyParameterFromForm]`.

Two traps, both hit and both fixed:

* **Do not add `<AntiforgeryToken/>`.** A static-SSR `EditForm` emits one already. Two hidden
  fields of the same name post as one comma-joined value and validation rejects the form with a
  bodyless 400 — which looks exactly like a blank page.
* **No circuit means no toasts, and no `@onclick`.** Login errors render inline (`.fh-form-error`)
  instead of through `MessageService`. `ThemeToggle` and `TextField`'s password reveal were
  rewritten as plain JS, which is a small win everywhere else too: both now work before the
  circuit has connected rather than sitting dead for a moment.

**Step 4 — `TokenProvider`.** **[changed]** Scoped, but it holds nothing and is seeded by nobody.
It asks `AuthenticationStateProvider` for the principal and reads the JWT claim off it, which
works identically in the HTTP render and in the circuit. Point
`AuthenticatedHttpClientService.AttachTokenAsync` at it instead of `AuthTokenService`.

**Step 5 — migrate the 14 call sites.** **[changed]** `AuthStateService` was reimplemented over
the `ClaimsPrincipal` rather than deleted, so most call sites did not have to move at all and the
"staff or author" rule stays in one place. `RoleGuard` is unchanged and now reads claims through
it. `MainLayout` and `NavMenu` lose their `OnAuthStateChanged` subscriptions — with the cookie
written and cleared by whole HTTP requests, a session can no longer begin or end part-way through
a circuit's life, so there is nothing to notify. Only `UserSettings` changed injection.

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

Followed the project's usual rule: a throwaway `postgres:17` container, never the live compose
stack. **All eight pass**, plus the two in §10.

1. ✅ `document.cookie` is the empty string — not merely missing the auth cookie, script can read
   nothing at all.
2. ✅ `localStorage` holds only `foodhub-theme` after login. No string starting `eyJ` anywhere
   script can reach.
3. ✅ Login lands on `/` (or the returnUrl); wrong password stays on the form showing
   "Ongeldig wachtwoord", no loop.
4. ✅ Logout clears both cookies and returns to `/login`. The API log shows
   `POST /api/authentication/signout → 204` *before* the cookie goes, which is the ordering that
   matters.
5. ✅ A role-less account is bounced from `/admin` to `/`, and the admin nav entry is absent.
6. ✅ Rotating the security stamp in the DB → next request traces
   `GET /recipes` → `GET /auth/logout` → `GET /login`, exactly the intended path.
7. ✅ Presence: the badge reads `--online`, and four heartbeats logged over two idle minutes.
8. ✅ `docker compose restart web` → still signed in, cookie still decrypts.

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

---

## 10. What §3 missed: `SameSite=Strict` breaks every link into the site

Not in the plan, and the most surprising part of the work. §4 chose `Strict` in one line without
noticing what it costs.

Strict means the auth cookie is **not sent on a navigation that starts on another site** — a link
in a mail, a chat, a search result. The session is not gone; that one request simply could not
prove it. But the request looks signed out, so the visitor lands on a login page while holding a
perfectly good cookie. Every shared link is affected.

**The fix: a second cookie that says only that a session exists.** `FoodHub.Session` holds `"1"` —
no identity, no token, nothing worth stealing — and is `SameSite=Lax`, so it *does* survive that
navigation. Lax next to an unauthenticated request means "reload and try again". The reload is
issued by the loaded document (`location.replace` in `App.razor`, `<noscript>` meta refresh
behind it), which makes it same-site, so the real cookie arrives and the URL never changes.

**It has to be the document that asks.** A server 302 does not work: a redirect inherits the
initiator of the navigation that caused it, so the second request would be cross-site as well and
get nothing.

**The loop guard is deleting the hint** before asking for the reload. If the reload comes back
authenticated the middleware writes it straight back; if it does not — the session really is over,
or the browser is refusing the Strict cookie outright — nothing is left to trigger a second
attempt and the request falls through to the login page. At most one extra load, ever.

### The login redirect had to move out of `MainLayout`

`MainLayout` redirected to a bare `/login`, losing wherever the visitor was going — a pre-existing
bug, nothing to do with cookies. Adding a `returnUrl` there did not work: the layout runs *in the
circuit*, which only exists after the page has been served, so it raced the static login page it
was navigating to. The trace showed `/login?returnUrl=%2Frecipes` arriving and then a bare
`/login` immediately overwriting it.

It is now `AuthCookie.UseLoginRedirect`, a plain 302 decided before any circuit exists, so there is
nothing to race — and a signed-out visitor no longer pays for a whole circuit before being turned
away. Order matters: `UseSessionHint` runs first and sets a flag that suppresses the redirect, so
a recoverable session gets its reload instead of being written off.

Return URLs are checked, not trusted — one leading slash, no second one (`//evil.example` and
`/\evil.example` both read as another host), or it falls back to `/`. Otherwise this would be an
open redirect: a link that genuinely starts on this site and ends on someone else's.

### Verified

9. ✅ Signed in, cross-site click on a link to `/recipes` → lands on the recipe page signed in.
   Exactly two GETs (the cookie-less one and the recovery reload), URL unchanged, and
   `location.replace` leaves no extra history entry.
10. ✅ Signed out, same click → `302 /login?returnUrl=%2Frecipes`, and signing in lands on
    `/recipes` rather than the home page. Query strings survive too:
    `/recipes/find?q=kip` round-trips intact.

### If Strict turns out to be more trouble than it is worth

Switch `options.Cookie.SameSite` to `Lax` in `Program.cs` and the hint cookie becomes dead weight.
But then `/auth/logout` must become a POST with an antiforgery token, including from inside the
circuit — which is the friction the Strict choice buys its way out of.
