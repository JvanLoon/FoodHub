# SignalR real-time updates: implementation plan

Status: plan only, no code changed. UI text in the app is Dutch; this document is English.
Domain terms (Recipe, Pending, Approved, Reject, Moderator, Admin) follow `GLOSSARY.md`.

## 1. Summary and decision

**Goal now:** (a) Review changes reach Admin/Moderator screens instantly (and the submitter's own page when
their item is approved); (b) login/logout (presence) changes reach Admin/Moderator screens instantly.
**Goal later:** an admin-only "notification" to all logged-in users (undefined, see section 10), plus other
"instant" features, cheaply.

**Decision: the hub lives in `FoodCalc.Api`. Each Blazor circuit in `FoodCalc.Web` opens an outbound,
server-to-server SignalR client connection to it, authenticated with the user's own JWT. Events received are
turned into plain C# events on a scoped service, and components call `InvokeAsync(StateHasChanged)` or re-fetch.**
The browser never talks to the hub; it keeps receiving UI diffs over the Blazor circuit it already has.

```
Browser --(Blazor circuit, WebSocket, via Cloudflare)--> web container
                                                           |  scoped RealtimeClient (per circuit)
                                                           |  HubConnection, ws://api:8080/hubs/events
                                                           |  Authorization: user's JWT (from cookie claim)
                                                           v
                                              api container: EventsHub  <--- IRealtimePublisher
                                                           ^                    (IHubContext<EventsHub,...>)
                                                           |                           ^
                                      MediatR notifications raised by handlers --------+
                                      (Approve/Reject/AddRecipe/..., Login/SignOut, presence sweeper)
```

Why not the alternatives: section 4. Safety net (slow poll + refresh on reconnect): section 8.

## 2. Current-state findings

**Topology and auth**
- `FoodCalc.Web/Program.cs`: Blazor Server, `AddInteractiveServerComponents()`. Browser auth is a cookie
  (`AuthCookie`, `FoodCalc.Web/Services/Auth/AuthCookie.cs`): HttpOnly, Secure, SameSite=Strict, 12 h, not sliding.
  The API's JWT is stored as an encrypted claim inside the cookie ticket (`AuthCookie.TokenClaim`); see
  `auth-cookie-migration.md` (root; there is no `docs/auth-cookie-migration.md`, `docs/` only holds `agents/`).
- `FoodCalc.Web/Services/Auth/TokenProvider.cs` reads the JWT from the circuit's `ClaimsPrincipal`;
  `AuthenticatedHttpClientService` attaches it as a Bearer header on calls to `API:BaseAddress` (in prod
  `http://api:8080`). A 401 triggers `AbandonSessionAsync` (navigate to the logout route).
- Web deliberately holds no `Jwt__Key` (HS256: key forges tokens; web is the exposed container). Web reads
  claims only (`AuthStateService`: Admin/Moderator/User via `ClaimTypes.Role`).
- `FoodCalc.Api/Program.cs`: FastEndpoints, MediatR (`AddApplicationMediatR` in `FoodCalc.Features/Extensions.cs`),
  EF Core/Npgsql, JWT bearer with `SecurityStampCheck.OnTokenValidatedAsync` (revokes on disable/role change/password
  reset), policies `"Admin,Moderator,User"`, `"Admin,Moderator"`, `"Admin"`, `"Moderator"`, `"User"`. CORS
  `AllowWebApp` (browser-origin only). `PresenceMiddleware` runs after auth, before endpoints.
- No automated test projects exist in the solution (`git ls-files | grep -i test` is empty). Verification is manual.

**Deployment** (`docker-compose.prod.yml`, `docker-compose.prod.quicktunnel.yml`, `deployment.md`)
- Only `cloudflared` -> `http://web:8080`. `api` and `db` publish no ports; api CORS origin is the public hostname.
- The stack uses Cloudflare Tunnel only (`deployment.md` has been corrected to say so).
- Both Program.cs files clear `KnownIPNetworks`/`KnownProxies` and use `X-Forwarded-For/Proto`; web calls
  `UseForwardedHeaders()` when `ReverseProxy:Enabled` (default on outside Development). Safe only because nothing else
  is published. A new internal `ws://api:8080` call from web needs no forwarded headers (plain internal HTTP).
- API runs `Database.MigrateAsync()` at startup; `deployment.md` already warns that a second API replica races it.
  API is therefore single-instance today.
- Target host is a Raspberry Pi (ARM, small RAM): keep per-connection cost low, no extra services (Redis) unless
  needed.

**Review (the places review state changes)**
- Queue UI: `FoodCalc.Web/Pages/Admin/ReviewQueue.razor` loads once in `OnInitializedAsync` -> `ReviewService.GetQueueAsync()`
  (`FoodCalc.Web/Services/ReviewService.cs`); there is no polling, so a moderator only sees new items on page load or after
  their own action (`RunAsync` reloads). Hosted in the Admin page tab set.
- API: `FoodCalc.Api/Endpoints/Review/ReviewEndpoints.cs` (queue GET, approve/reject recipe, approve/reject recipe line;
  policy `Admin,Moderator`). Handlers: `FoodCalc.Features/Review/Commands/ApproveContent/ApproveContentCommandHandler.cs`,
  `.../RejectContent/RejectContentCommandHandler.cs`; query `Queries/GetReviewQueue`.
- Transitions **to Pending** (all set `IsReviewed = false`):
  `Recipes/Commands/AddRecipe`, `AddIngredientToRecipe` (lines 50, 51, 63, 64), `UpdateRecipe` (line 64),
  `UpdateRecipeName` (line 34), `Ingredients/Commands/DeleteIngredientFromRecipe` (line 31),
  `Ingredients/Commands/AddIngredient` (line 32; ingredient-level flag). Also relevant to the queue without a flag change:
  `Recipes/Commands/DeleteRecipe` (an author deleting a Pending recipe removes it from the queue) and
  `Ingredients/Commands/DeleteIngredient`/`UpdateIngredient` (verify when implementing whether they touch queue content).
- Transitions **out of Pending**: `ApproveContentCommandHandler` (recipe approve also approves its lines and ingredients;
  line approve only clears the line's flag, recipe stays Pending), `RejectContentCommandHandler` (recipe reject deletes it;
  line reject deletes the line).
- Submitter view: `FoodCalc.Web/Pages/Recipes/EditRecipe.razor` shows the "Ingediend ter goedkeuring" banner
  (`WebConstants.Messages.Review.PendingApproval`) while `_recipe.Id != Guid.Empty && !_recipe.IsReviewed`; it loads the
  recipe in `OnParametersSetAsync` via `RefreshRecipeAsync`, with no live update.

**Presence**
- Write path: `PresenceMiddleware` (after each authenticated response, throttled by singleton `Common/PresenceThrottle.cs`,
  45 s) -> `TouchPresenceCommandHandler` (`ExecuteUpdate`, else insert). `PresenceWindow` (`Features/Authentication/Presence`):
  `Timeout` 3 min, `WriteInterval` 45 s.
- **Login** event point: `Endpoints/Authentication/LoginEndpoint.cs` (~line 92, `TouchPresenceCommand(user.Id)` after success).
  **Logout** event point: `Endpoints/Authentication/SignOutPresenceEndpoint.cs` (`TouchPresenceCommand(userId, IsOnline:false)`),
  called from `FoodCalc.Web/Services/Auth/PresenceService.SignalOfflineAsync` in `MainLayout.Logout`.
  **Implicit logout**: browser/tab closed, JWT rejected (`AbandonSessionAsync` sends no ping), or lost network: nothing is
  written; the dot only goes out when `LastSeenUtc` ages past 3 min. There is currently no server component that notices this.
- `PresenceService` (scoped, Web) heartbeats every 60 s via `POST` heartbeat; started in `MainLayout.OnInitializedAsync`.
- Admin read side: `GetPresenceEndpoint` / `GetPresenceQueryHandler` (max 200 ids), polled every 30 s by
  `FoodCalc.Web/Pages/Admin/UserList.razor` (`PollInterval`, `PollPresenceAsync`) through `AdminService`.
- Presence is stateful only in DB (`UserPresence`) plus the in-memory throttle.

## 3. Architecture

Components (names are proposals; place per the existing vertical-slice conventions):

| Project | Addition |
|---|---|
| `FoodHub.DTOs` | `Realtime/` event contract (records), `RealtimeRoutes`/hub path constant and method-name constants (same pattern as `ApiRoutes.cs`). |
| `FoodCalc.Features` | `Realtime/IRealtimePublisher` (abstraction), `Realtime/RealtimeAudience`, MediatR `INotification` types raised from handlers. No SignalR dependency here. |
| `FoodCalc.Api` | `Hubs/EventsHub : Hub<IEventsClient>`, `Realtime/SignalRRealtimePublisher`, notification handlers that call the publisher, `PresenceSweeperService` (BackgroundService), hub mapping + auth config. |
| `FoodCalc.Web` | `Services/Realtime/RealtimeClient` (scoped, per circuit), `Microsoft.AspNetCore.SignalR.Client` package, component subscriptions. |

Rules of the design:
1. **Events are invalidation signals, not data.** Payload = what changed + ids + minimal display text; clients re-fetch
   through the existing authorised API calls. This keeps role enforcement in one place (API endpoints) and makes
   payloads safe even if misdelivered.
2. **Typed hub** (`Hub<IEventsClient>`): one strongly typed client interface; adding an event = one interface method + one DTO.
3. **Groups by audience:** `role:Admin`, `role:Moderator`, `role:User`, `user:{userId}`. A connection is placed in groups
   in `OnConnectedAsync` from its validated JWT claims, never from client-supplied arguments. Clients cannot join groups
   (no hub methods that accept group names). The hub has **no client-callable methods** initially (server push only).
4. **Publisher abstraction** `IRealtimePublisher.PublishAsync<TEvent>(TEvent evt, RealtimeAudience audience)`; the only code
   that knows about `IHubContext`. Swapping to a backplane or a no-op in tests is one registration.
5. **Raised after commit, best effort:** a publish failure is logged and swallowed (same philosophy as
   `TouchPresenceCommandHandler`). The real state is always the DB.

## 4. Decision record: where does the hub live?

Constraint recap: browser reaches only `web`; `api` has no published port and must stay that way (the signing key lives
only there); `web` is deliberately credential-poor.

| Option | Verdict |
|---|---|
| **A. Hub in API; each Web circuit is a SignalR client; Web fans out in-process (recommended)** | Works with current network layout (outbound from web to api, same direction as today's HTTP calls). Reuses JWT validation + `SecurityStampCheck` unchanged. API enforces groups from verified claims. No new secret, no new exposed surface. Cost: one extra internal WebSocket per open circuit (cheap on the Docker network). Reconnect logic is ours (`WithAutomaticReconnect`) and must handle token expiry. |
| B. Hub in Web; API pushes to it | Needs API -> Web delivery: an internal HTTP endpoint on web, which is the *exposed* process (the tunnel forwards every path to `web:8080`), so it needs path blocking/secret auth, and it adds a credential on the exposed side. Or Postgres LISTEN/NOTIFY: Web would need DB credentials (it has none today; it would undo the isolation). Browser-facing hub would also duplicate what the circuit already gives. Rejected. |
| C. Browser connects directly to hub in API | Not viable: API unreachable from the internet; exposing it would break the isolation model and need CORS+token in browser (token is deliberately never in the browser). Rejected. |
| D. Hub in Web, Web polls API for changes | Just polling with extra steps; no instant delivery. Used only as the *fallback* (section 8). |
| E. Single shared service connection Web -> API (one HubConnection, fan-out to circuits) | Fewer sockets, but needs a new shared service secret and moves audience filtering into Web (the less trusted process). Keep as a later optimisation if circuit counts get large. Design keeps `RealtimeClient` behind an interface so it can be swapped without touching components. |

Trade-offs of A:
- **Auth:** the JWT (12 h, same lifetime as the cookie) is passed via `HubConnectionBuilder.WithUrl(..., o => o.AccessTokenProvider = ...)`.
  Over WebSockets it travels as `?access_token=` on the internal docker network only; API must not log query strings for
  that path (check Serilog/request logging; today default logging only). Hub endpoint requires policy `"Admin,Moderator,User"`.
  `SecurityStampCheck` runs on connect/negotiate; a disabled user is not kicked from an already-open hub connection until
  reconnect. Acceptable (they also lose the Blazor session on the next API 401); optionally the API sends a `SessionRevoked`
  event on disable/role change to close connections (nice-to-have).
- **Reconnects:** `WithAutomaticReconnect` with backoff (0, 2, 5, 10, 30 s, then 30 s forever via custom `IRetryPolicy`); on
  `Reconnected` and on first `Closed`-then-start, raise `Resynced` so components re-fetch (missed events).
- **Multi-instance (future):** with one API replica, in-memory groups are enough. Two API replicas would deliver an event only
  to clients connected to the replica that raised it. Fix is localised to `IRealtimePublisher`'s implementation: add a SignalR
  Redis backplane, or a Postgres LISTEN/NOTIFY relay inside the API (no new service; Npgsql already present), plus move the
  presence "known online" set from memory to DB. Multiple web replicas are fine (each circuit has its own connection; Blazor
  already needs sticky sessions). Also fix migrations first (`deployment.md` known issue).
- **Cloudflare Tunnel:** WebSockets are supported by default on tunnels; Blazor circuits already depend on it. The new
  connections are internal (web->api) and do not touch Cloudflare. No change needed.
- **Raspberry Pi:** an idle SignalR connection costs tens of KB plus a 15 s keep-alive ping. Limit it: only connect circuits
  that need events (Phase 1/2: Admin/Moderator; Phase 3 later: everyone). Do not start the connection for anonymous/login
  layouts. Set `ServerTimeout`/`KeepAliveInterval` defaults, `MaximumReceiveMessageSize` small, no per-event DB queries
  at publish time.
- **Latency:** event is published right after `SaveChangesAsync`; delivery is typically tens of ms.

## 5. Event contract (`FoodHub.DTOs/Realtime`)

Envelope-free: each event is its own record; the hub interface carries one method per event (typed hub).
All include `DateTime OccurredUtc`. No emails/passwords/tokens in payloads where avoidable; user display uses existing id
and the clients re-fetch details.

```csharp
public interface IEventsClient   // FoodCalc.Api/Hubs, uses DTO records
{
    Task ReviewQueueChanged(ReviewQueueChangedEvent e);   // audience: role:Admin, role:Moderator
    Task RecipeReviewResolved(RecipeReviewResolvedEvent e); // audience: user:{authorId}
    Task UserPresenceChanged(UserPresenceChangedEvent e);  // audience: role:Admin, role:Moderator
    // Phase 3 placeholder, shape NOT decided (section 10):
    // Task NotificationReceived(...);
}
```

| Event | Payload | Raised when | Audience |
|---|---|---|---|
| `ReviewQueueChangedEvent` | `ReviewChangeKind Kind` (`Submitted`, `Edited`, `LineChanged`, `Approved`, `Rejected`, `LineApproved`, `LineRejected`, `Withdrawn`), `Guid RecipeId`, `string RecipeName`, `int PendingCount` (optional, cheap COUNT), `OccurredUtc` | Any transition to or out of Pending (list in section 2) | `role:Admin`, `role:Moderator` |
| `RecipeReviewResolvedEvent` | `Guid RecipeId`, `RecipeReviewOutcome Outcome` (`Approved`, `Rejected`, `LineRemoved`), `OccurredUtc` | Approve/reject recipe, reject line | `user:{CreatedByUserId}` only |
| `UserPresenceChangedEvent` | `string UserId`, `bool IsOnline`, `DateTime? LastSeenUtc`, `PresenceReason Reason` (`Login`, `Logout`, `HeartbeatResumed`, `TimedOut`), `OccurredUtc` | Offline->online transition, explicit sign-out, sweeper timeout | `role:Admin`, `role:Moderator` |
| `Resynced` (client-local, not hub) | none | `RealtimeClient` reconnected | n/a |

Raising mechanism: handlers in `FoodCalc.Features` publish MediatR `INotification`s (e.g. `ReviewStateChanged`,
`UserPresenceTransitioned`) via `IMediator.Publish` after `SaveChangesAsync`; `FoodCalc.Api` holds the
`INotificationHandler<>`s that map them to `IRealtimePublisher` calls. This keeps `Features` free of SignalR and keeps the
handlers' diffs small (one `await mediator.Publish(...)` line each). Notification handlers must not throw into the command
(wrap in try/catch, log warning).

## 6. Phased implementation

Each phase is independently shippable. After each phase the app must behave exactly as before for users without the
feature, and the fallback poll (section 8) must already exist for that phase's screens.

### Phase 0: infrastructure and spike (about 0.5 to 1 day)
1. `FoodHub.DTOs`: add `Realtime/RealtimeRoutes.cs` (hub path `/hubs/events`) and the empty event folder; add hub method-name
   constants only if not using the typed client's methods directly from the Web side (Web client uses `connection.On<T>("MethodName", ...)`,
   so constants avoid string drift).
2. `FoodCalc.Features`: `Realtime/IRealtimePublisher`, `RealtimeAudience` (`Roles(params string[])`, `User(string id)`, `All()`),
   `NullRealtimePublisher` default registration so Features/test code never needs the API.
3. `FoodCalc.Api`:
   - `builder.Services.AddSignalR()`; `Hubs/EventsHub : Hub<IEventsClient>` with `[Authorize(Policy = "Admin,Moderator,User")]`;
     `OnConnectedAsync` adds groups `role:{r}` for each role claim and `user:{sub}`.
   - JWT bearer: in `JwtBearerEvents` add `OnMessageReceived` to read `access_token` from the query **only when the path starts with
     `/hubs/events`** (merge with existing `OnTokenValidated`).
   - `app.MapHub<EventsHub>(RealtimeRoutes.EventsHub)` after `UseAuthorization()` (CORS not needed: no browser origin).
   - `SignalRRealtimePublisher` (singleton, `IHubContext<EventsHub, IEventsClient>`), registered over the null one.
   - Spike endpoint (Development only) `POST /api/dev/realtime-ping` publishing a test event, so the pipe can be proved end to end.
4. `FoodCalc.Web`: add package `Microsoft.AspNetCore.SignalR.Client` (match 10.0.x). `Services/Realtime/RealtimeClient`
   (scoped, `IAsyncDisposable`):
   - `StartAsync()` builds `HubConnection` to `API:BaseAddress + RealtimeRoutes.EventsHub`, `AccessTokenProvider` = `TokenProvider.GetTokenAsync`.
   - Exposes typed C# events (`ReviewQueueChanged`, `RecipeReviewResolved`, `UserPresenceChanged`, `Resynced`), each raised
     on the circuit via the component's own `InvokeAsync`.
   - Custom retry policy; on `Reconnected` raise `Resynced`; on token rejection (401 on negotiate) do **not** loop forever:
     stop and let the existing 401 handling (`AbandonSessionAsync`) own session termination.
   - Registered `AddScoped<RealtimeClient>()` in `FoodCalc.Web/Program.cs`; started from `MainLayout.OnInitializedAsync` next to
     `PresenceService.Start()` and only when `AuthStateService.IsInAnyRoleAsync("Admin","Moderator")` (Phase 3 widens this to all roles).
   - Disposed with the circuit; `Logout()` in `MainLayout.razor` calls `StopAsync` before navigating (alongside `SignalOfflineAsync`).
5. Spike check: a throwaway debug component (not committed) shows events. Confirm connect, reconnect (stop/start `api`), and
   WebSocket negotiation in the docker stack. Decide `UseSkipNegotiation`/WebSockets-only (`HttpTransportType.WebSockets`) for the
   internal client to avoid long-polling fallbacks.

Exit: ping appears in a second logged-in Moderator session within about 1 s; unauthenticated connect gets 401; a plain `User`
cannot receive moderator-audience events.

### Phase 1: review events (about 1.5 to 2 days)
1. Features: `Review/Events/ReviewStateChanged : INotification` (kind, recipeId, name, authorUserId). `await mediator.Publish(...)` after
   `SaveChangesAsync` in: `AddRecipeCommandHandler` (Submitted), `AddIngredientToRecipeCommandHandler`, `UpdateRecipeCommandHandler`,
   `UpdateRecipeNameCommandHandler`, `DeleteIngredientFromRecipeCommandHandler` (Edited/LineChanged, **only when `IsReviewed`
   actually flipped or a changed line was added**, to avoid noise), `DeleteRecipeCommandHandler` (Withdrawn),
   `ApproveContentCommandHandler` (Approved / LineApproved), `RejectContentCommandHandler` (Rejected / LineRemoved),
   plus `AddIngredientCommandHandler` if it results in Pending ingredient state visible in the queue (verify).
   Handlers need the recipe name and author id; they already load the recipe.
2. Api: `Realtime/ReviewStateChangedHandler` maps to `ReviewQueueChangedEvent` (audience `Roles("Admin","Moderator")`) and, for
   Approved/Rejected/LineRemoved, `RecipeReviewResolvedEvent` (audience `User(authorId)`). Skip self-notification: include
   `ActorUserId` (see `FoodCalc.Features/Review/ActingUser.cs`) so the moderator's own tab need not refetch twice (optional).
3. Web:
   - `ReviewQueue.razor`: implements `IDisposable`; subscribes to `RealtimeClient.ReviewQueueChanged` and `Resynced`; handler does a
     debounced (250 to 500 ms, coalescing bursts) `LoadAsync()` then `InvokeAsync(StateHasChanged)`. Skip reload while `busy`
     (run it once when `busy` clears). Optional toast `UiText.Review.NewSubmission` (Dutch text in `UiText.cs`).
   - Pending-count badge on `Admin.razor` / `NavMenu.razor`: **not now** (user: maybe later). Keep `PendingCount` in the payload only.
   - `EditRecipe.razor`: subscribe to `RecipeReviewResolved` filtered by `RecipeId == _recipe.Id`; call `RefreshRecipeAsync()` so the
     "Ingediend ter goedkeuring" banner disappears on approval; on `Rejected` navigate to the recipe list with a Dutch message.
     This requires the author's circuit to hold a connection. **Decision (user): do this only if it is cheap, otherwise skip it.**
     Cheap means: `RealtimeClient.StartAsync` is started on demand (ref-counted) by `EditRecipe` only, with no change for other pages,
     and `RecipeReviewResolved` is the only extra event. If that turns out to need more than about half a day or touches many
     components, drop this bullet and ship the moderator side only (the author still sees the status on the next load).
     Do not widen the connection to all users in Phase 1.
   - Fallback poll: `ReviewQueue` re-fetches every 60 s while visible (cheap `GET`; reuses existing endpoint) and on `Resynced`.
4. Verification: section 9.

Exit: submit a recipe as User in session A; the Moderator's open queue in session B shows it within about 1 s without reload;
approving it in B clears A's banner within about 1 s.

### Phase 2: presence events (about 1.5 days)
1. Features: `Authentication/Presence/Events/UserPresenceTransitioned : INotification` (userId, isOnline, lastSeen, reason).
   `TouchPresenceCommandHandler` must know the *previous* state to raise a transition. Cheapest correct approach: add a singleton
   `PresenceTracker` in the API (in-memory `ConcurrentDictionary<string, DateTime>` of users currently online) consulted in
   `PresenceMiddleware`/Login/SignOut:
   - First sighting (not in tracker or lapsed) -> publish `Login` (from `LoginEndpoint`) or `HeartbeatResumed` (middleware).
   - `SignOutPresenceEndpoint` -> remove from tracker, publish `Logout`.
   - Throttle interaction: `PresenceThrottle` already limits DB writes; the tracker check is an O(1) dictionary lookup and adds no DB cost.
   - On API restart the tracker is empty: the first heartbeat of every user would then emit `HeartbeatResumed` (a harmless burst
     once). Seed the tracker at startup from `UserPresence` rows with `IsOnline` and recent `LastSeenUtc` (`PresenceWindow.IsOnline`)
     to avoid it. (With multiple API instances this moves to the DB; see section 4.)
2. Api: `PresenceSweeperService : BackgroundService` (every 15 to 30 s, `PeriodicTimer`): compares tracker against
   `PresenceWindow.Timeout` and publishes `TimedOut` for users whose last seen is older, removing them from the tracker. This is
   the only way to get an instant-ish "went offline" for closed tabs. Does not write to the DB (the DB already says "online but
   stale", which `PresenceWindow.IsOnline` treats as offline). Accepting up to 3 min + sweep interval for implicit offline is
   inherent to the heartbeat model. **Open decision, ask the user when Phase 2 starts (do not pre-decide):** whether to shorten this
   with a faster "went offline on tab close", e.g. `RealtimeClient` disconnect (circuit closed) calling the existing
   `SignOutPresence` after a 20 to 30 s grace period (a tab refresh must not flicker a user offline). Present the trade-off and let
   the user choose; skip it entirely if they have no preference.
3. Api: notification handler maps to `UserPresenceChangedEvent` (audience `Roles("Admin","Moderator")`). **Decision (user): Admins and
   Moderators are treated the same for now.** This matches `GetPresenceEndpoint`, which already has policy `Admin,Moderator`.
4. Web: `UserList.razor` (the admin "Users" page) subscribes to `UserPresenceChanged` and updates the matching row's
   `IsOnline`/`LastSeenUtc` in place (only if the user is on the current page; ignore others), then `InvokeAsync(StateHasChanged)`.
   Keep the existing 30 s `PollPresenceAsync` as the safety net (lengthen to 60 s). No toast or dashboard widget.
5. Edge cases: same user in two tabs (second tab is not a transition); user disabled (`ToggleUserEndpoint`) or deleted
   (`DeleteUserEndpoint`): publish `Logout`/`TimedOut` so the dot clears.

Exit: log in as User in session A, Admin's user list in session B flips the dot within about 1 s; Uitloggen flips it within about 1 s;
closing the tab A flips within ~3.5 min (or sooner if the optional disconnect path is done).

### Phase 3: notifications (placeholder, gated on section 10)
Do not start until the user has answered the questions in section 10. Preparation already done by Phases 0 to 2: the typed hub, per-role
and per-user groups, publisher abstraction and the client service. Expected shape, pending answers: `NotificationReceived`
event method + DTO in `FoodHub.DTOs/Realtime`, an Admin-only send page (`Policies("Admin")` endpoint), `RealtimeClient` widened to start
for every role in `MainLayout`, and a Web host component (likely next to `ToastHost`). Persistence, read state, expiry and targeting
depend entirely on the answers. Effort cannot be estimated until then.

## 7. Auth and security

- Hub requires JWT via the existing scheme; policy `"Admin,Moderator,User"`; groups assigned server-side from claims only.
- Moderator/Admin-only events go only to `role:Admin`/`role:Moderator` groups; per-author events to `user:{id}`. Never broadcast
  review or presence data to `All`.
- Payloads are invalidation signals plus ids and a recipe name; no emails, tokens or other users' private data. The client re-fetches
  through endpoints that already enforce policy (`GetReviewQueueEndpoint`, `GetPresenceEndpoint`). A misrouted event therefore leaks
  at most "a recipe named X changed".
- Token handling: JWT read from the circuit's principal (`TokenProvider`); in query string only on the internal docker link; do
  not log it. The browser never sees it (unchanged). Restrict `OnMessageReceived` query-token handling to the hub path.
- No client-callable hub methods in Phase 0 to 2, so no group-join or message-injection surface. Set `HubOptions.MaximumReceiveMessageSize`
  small.
- Token expiry: JWT and cookie both expire at 12 h; the client treats a negotiate 401 as session end (reuse `AbandonSessionAsync`
  semantics), not as a reason to retry.
- CORS: not required for web->api server-to-server SignalR (CORS applies to browsers); keep `AllowWebApp` unchanged.
- Revocation: `SecurityStampCheck` is applied at connect. Optional `SessionRevoked` event on disable/role change to force
  `HubConnection.StopAsync` (and role-group refresh on role change, since groups are fixed at connect).
- Role changes mid-session: group membership is fixed at connect; a demoted moderator keeps receiving invalidation signals until
  reconnect, but any re-fetch fails with 403 from the API. Acceptable; mitigate with the optional `SessionRevoked`.

## 8. Fallback and safety net

- **Refresh on reconnect:** `RealtimeClient` raises `Resynced` after `Reconnected` or a successful restart; subscribed pages re-fetch.
- **Slow poll stays:** `ReviewQueue` polls every 60 s (new); `UserList` keeps its existing 30 s presence poll (relax to 60 s once events
  work). Pages that subscribe must keep the poll so a dead connection or a dropped event never leaves a screen stale for more than
  a minute (this matches the user's stated acceptable bound).
- **Feature flag:** `Realtime:Enabled` (config, default true) disables hub mapping and makes `RealtimeClient.StartAsync` a no-op, so a
  production issue can be mitigated by an environment variable and restart, falling back to polling only.
- **Failure isolation:** publisher and notification handlers swallow and log; commands never fail because of SignalR.

## 9. Testing and verification

There are no test projects today. **Decision (user): a test project may be added, but only for the SignalR work.** Tests for the rest of
the app are a separate, later stage and must not be added here. Add `FoodCalc.Tests` (xUnit) containing only:
- publisher mapping tests (notification -> event/audience), using a fake `IRealtimePublisher`;
- `PresenceTracker`/sweeper transition logic with a fake clock;
- handler tests only where they already have in-memory EF patterns (none exist; keep optional and limited to SignalR-related code).

Manual verification (in-app browser, two sessions). Use Aspire (`FoodHub.AppHost`) or `docker compose` locally; log in with bootstrap/seed
accounts (do not paste credentials into chat).
1. Open session 1 in a normal tab as Moderator/Admin on Admin > Review queue; open session 2 in a *separate browser profile/
   incognito* (the cookie is per browser) as a plain User.
2. Session 2 creates a recipe or adds an ingredient to an Approved recipe. Expect session 1's queue to update in about 1 s (no reload).
3. Session 1 approves; session 2, on `EditRecipe`, loses the "Ingediend ter goedkeuring" banner. Repeat with reject (session 2 redirected).
4. Presence: session 1 on Admin > Gebruikers; log in/out as the User in session 2; dot flips in about 1 s. Close session 2's tab without
   signing out; expect it off within about 3.5 min (or the optional faster path).
5. Resilience: stop/start the `api` container while session 1 is open; hub reconnects, `Resynced` triggers a refresh; verify no
   duplicate rows and no console error loop. Also block the hub connection (stop api) and confirm the 60 s poll still works after restart.
6. Security: as a plain User, confirm no moderator events arrive (log in `RealtimeClient` at Debug); unauthenticated hub negotiate returns 401.
7. Check browser console, `docker compose logs web api` for exceptions, memory (`docker stats`) with 10 open circuits.
8. Prod-like: run `docker-compose.prod.yml` or the quicktunnel variant and repeat 2 to 3 through the real Cloudflare hostname.

## 10. Open question: what is a notification?

**This is deliberately undefined. The implementing agent MUST ask the user the following questions, and wait for answers, before
implementing Phase 3 or designing any notification DTO, table or UI. Do not guess answers.**

1. Presentation: transient toast (like `ToastHost`), a persistent inbox/bell with unread count, a blocking banner/modal, or several?
2. Storage: ephemeral (only delivered to currently connected users) or persisted in the database (new table, migration)?
3. Offline users: should someone who is offline at send time still get it later (on next login)? For how long?
4. Read state: per-user read/dismissed tracking? Can a user dismiss it? Is "who has read it" visible to the admin?
5. Targeting: all logged-in users only, or also by role, specific users, or all registered users (including offline)?
6. Content: plain text only, or title + body, links, formatting (careful with XSS; see `xss-hardening-plan.md`), severity level?
7. Lifetime: expiry/auto-dismiss time? Can the admin edit or retract a sent notification?
8. Delivery guarantee: best effort is acceptable, or must delivery be confirmed/acknowledged?
9. Localisation: Dutch only (like the rest of the UI), or per-user language?
10. Admin UI: history of sent notifications, scheduling for later, templates, rate limiting or confirm-before-send?
11. Do Moderators send or only Admins? Should sends be audited (who/when)?
12. Does it need to survive a deployment of many API/web instances (backplane requirement) at that point?

## 11. Deployment and ops changes

- **Compose (`docker-compose.prod.yml`, `docker-compose.prod.quicktunnel.yml`, `docker-compose.yml`):** none required for networking.
  `api` still publishes no ports; `web` already reaches it at `http://api:8080` via `API__BaseAddress`. Optional env: `Realtime__Enabled=true`
  (both services). Add a Docker healthcheck is out of scope.
- **Tunnel/proxy:** no change; Cloudflare Tunnel already carries WebSockets for Blazor.
- **ForwardedHeaders:** unchanged. The new connection is internal and does not need or trust forwarded headers. Keep the existing warning:
  never publish `api` or `web` ports directly.
- **Config:** `API:BaseAddress` is reused for the hub URL. No new secrets. If the shared-connection option E is ever adopted, a service secret
  would be added to `.env.example` and both services.
- **Raspberry Pi:** no new containers. Expect small RAM growth per open privileged circuit; cap by connecting only roles that need
  it (Phase 0/1 decision), and confirm with `docker stats`.
- **Docs to update with the change:** `deployment.md` (hub + internal WebSocket note), `README_api.md` (hub path, events),
  `GLOSSARY.md` only if a new domain term appears (e.g. "Notification" after section 10 is answered). Consider an ADR in `docs/adr/`
  recording the section 4 decision (the folder does not exist yet).
- **Scaling note:** single API replica. Before a second replica: split migrations out of startup, add backplane (Redis or Postgres
  LISTEN/NOTIFY), move `PresenceTracker` state to DB.

## 12. Risks and open questions

Risks
- Handler coverage: missing one "to Pending" transition leaves the queue stale until the 60 s poll. Mitigate with the checklist in
  section 2 and a grep review for `IsReviewed =` at the end of Phase 1.
- Event storms (bulk import via `ImportExportService`, many line edits): debounce on the client; publish one event per command; consider
  suppressing events during import (`DataImportExport.razor` path).
- Token in query string for the internal hub WebSocket: acceptable on the private docker network; confirm no request logging captures it.
- Per-circuit connections scale with users; revisit option E if the Pi struggles.
- Blazor re-render rules: event callbacks arrive on a non-circuit thread; always `InvokeAsync(...)` and unsubscribe in `Dispose` to avoid
  leaks and "disposed circuit" exceptions.
- Presence tracker is in memory: restart bursts and multi-instance inconsistency (documented, seeded at startup).

Decisions made by the user (answers to the earlier open questions)
1. **Presence audience:** Admins and Moderators are treated the same for now. The admin "Users" page (`UserList.razor`) updates via events.
2. **Submitter's page updates live:** only if cheap (on-demand connection from `EditRecipe` only, see Phase 1); otherwise skipped.
3. **Faster "went offline on tab close":** undecided. **Ask the user at the start of Phase 2** what they want; do not assume.
4. **Pending-count badge on the Admin nav item:** not now, maybe later. Do not build it; keep `PendingCount` in the event payload so it is cheap to add.
5. **Test project:** allowed, but only for the SignalR work (see section 9). Tests for the rest of the app come in a different stage.
6. **Docs:** the stack is Cloudflare Tunnel; `deployment.md` has been corrected to say so.

Still open
- Section 10 questions (what a notification is): ask at the start of Phase 3.
- Question 3 above: ask at the start of Phase 2.

## 13. Rough effort

| Phase | Effort |
|---|---|
| 0 Infrastructure + spike | 0.5 to 1 day |
| 1 Review events (+ submitter banner, poll fallback) | 1.5 to 2 days |
| 2 Presence events (+ tracker, sweeper) | 1.5 days |
| Docs, ADR, manual verification passes | 0.5 day |
| 3 Notifications | not estimable until section 10 is answered |

Total for phases 0 to 2: about 4 to 5 days of focused work.
