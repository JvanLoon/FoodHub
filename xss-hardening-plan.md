# Plan: Content-Security-Policy, then sanitisation if user HTML ever gets rendered

Status: **not started**. Written to be handed to a fresh session.

Two parts. **Part A is work to do now.** **Part B is a tripwire** — there is nothing to sanitise
today, and the plan says what to do on the day that changes.

Both target XSS at the cause, rather than at the consequence the way
[auth-cookie-migration.md](auth-cookie-migration.md) does. Do these first: they are cheaper, they
force no logout, they touch no auth code, and a CSP also blocks the click-synthesis attack that an
httpOnly cookie does nothing about.

---

# Part A — Content-Security-Policy

## A1. Why this is cheap here

The app is unusually well placed for a strict policy:

* Every script is local — `js/interop.js`, `_framework/blazor.web.js`, vendored Bootstrap. No CDN.
* No `MarkupString` anywhere in the codebase.
* No `innerHTML` / `document.write` in your own JS.
* No `data:` image URIs, no externally-hosted images or fonts.
* **Exactly one inline `<script>`** — the theme block in `FoodCalc.Web/App/App.razor:8`.
* Only two inline `style=` attributes (`Pages/Admin/Home.razor:130`,
  `Pages/Calendar/MealCalendar.razor:190`).

So `script-src 'self'` is achievable with one small refactor, and that is the setting that matters.

## A2. The single blocker, and how to clear it

`App.razor` runs an inline script to apply the saved theme before first paint. Inline scripts are
exactly what `script-src 'self'` forbids.

Three ways out, in order of preference:

1. **Move it to `wwwroot/js/theme-init.js` and reference it with `@Assets[...]`.** *Recommended.*
   A blocking `<script src>` in `<head>` still runs before first paint, so the anti-flash
   behaviour is preserved. It gets fingerprinted and cached like the other assets, needs no
   per-request plumbing, and leaves the policy as a clean `script-src 'self'`.
2. A `sha256-` hash of the script body in the policy. Works, but any edit to the script silently
   breaks the page until the hash is updated — a nasty trap.
3. A per-request nonce. Cleanest in theory, but the nonce has to reach `App.razor`, which means
   `IHttpContextAccessor` plumbing during the static render. Not worth it for one script.

**Take option 1.**

## A3. Proposed policy

```
default-src 'self';
script-src 'self';
style-src 'self' 'unsafe-inline';
img-src 'self' data:;
font-src 'self';
connect-src 'self';
base-uri 'self';
object-src 'none';
frame-ancestors 'none';
form-action 'self';
upgrade-insecure-requests
```

Notes on the deliberate choices:

* **`style-src` keeps `'unsafe-inline'`.** The two inline styles include a dynamic one
  (`style="width:@(pct)%"`) which cannot be hashed, and Blazor injects its own inline styles for
  the reconnection UI. Inline *style* is a far weaker vector than inline *script* — it is worth
  accepting to get `script-src 'self'` clean. Do not let it block the rest.
* **`connect-src 'self'`** must cover the SignalR WebSocket. Same-origin `wss:` matches `'self'`
  in modern browsers; confirm in Report-Only rather than assuming.
* **No `'unsafe-eval'`.** That is a Blazor *WebAssembly* requirement; Blazor **Server** should not
  need it. Confirm in Report-Only.
* **`frame-ancestors 'none'`** supersedes `X-Frame-Options`.

Worth adding alongside, all one-liners:

```
X-Content-Type-Options: nosniff
Referrer-Policy: strict-origin-when-cross-origin
Permissions-Policy: geolocation=(), microphone=(), camera=()
```

## A4. Rollout — do not skip this

**Ship `Content-Security-Policy-Report-Only` first.** It reports violations without breaking
anything, which is how you find whatever this plan got wrong before users do.

1. Deploy Report-Only.
2. Exercise every page — login, recipes, calendar, admin tabs, import/export, the file download
   (`blazorDownloadFile`), theme toggle, and a deliberate circuit drop to trigger the reconnect UI.
3. Read violations from the browser console (a `report-uri` endpoint is optional and probably
   overkill here).
4. Only when it is quiet, rename the header to `Content-Security-Policy`.

## A5. Steps

1. Move the theme script to `wwwroot/js/theme-init.js`; reference via `@Assets[...]` in `<head>`.
   Verify no theme flash on load, in both themes.
2. Add a small middleware in `FoodCalc.Web/Program.cs` setting the header. Place it early — just
   after `UseForwardedHeaders()` — so it covers static assets and error responses too. Keep the
   policy in one `const` string.
3. Ship as **Report-Only**. Walk the app per A4.
4. Flip to enforcing. Commit separately, so reverting is one commit.
5. Add the three companion headers in the same middleware.

The API needs none of this — it is internal-only and serves no HTML.

## A6. Gotchas

* **Cloudflare can inject scripts.** Rocket Loader and Web Analytics both do, and a strict
  `script-src 'self'` will break them (or they will break the page). Check the dashboard for the
  tunnel before enforcing — routing already lives there.
* **The reconnect UI** only appears when a circuit drops. Easy to miss in testing; force it by
  stopping the server briefly with the page open.
* **`MapStaticAssets` fingerprinting** is unaffected — same-origin either way.

## A7. Verification

* Every page loads clean with **zero** CSP violations in the console.
* Theme applies before first paint, no flash, light and dark.
* SignalR connects — interactivity works (click a button, see it respond).
* Reconnect UI renders when the circuit drops.
* Export/download still works.
* A deliberately injected `<script>alert(1)</script>` via an inline `<script>` in the DOM is
  **blocked** — that is the proof the policy is live.

---

# Part B — Sanitisation (tripwire, not work)

## B1. There is nothing to sanitise today

Checked the entities: `Recipe` has `Name` only. `Ingredient` has `Name` only. **Neither has a
description, instructions or notes field**, and nothing in the app renders user content as HTML.

Razor auto-encodes interpolated content, so `@recipe.Name` is already safe even if someone submits
`<script>alert(1)</script>` — it renders as visible text. **Do not add a sanitiser now.** It would
be code with no caller, and sanitising plain text that is already encoded on output only risks
mangling legitimate input (an ingredient genuinely called `Jam & Co` should stay that way).

## B2. The tripwire

Act on this the moment **any** of these appear:

* A `Description`, `Instructions`, `Notes` or `Steps` field on `Recipe` or `Ingredient`.
* `MarkupString` used anywhere in `FoodCalc.Web`.
* A markdown renderer, WYSIWYG editor, or "rich text" of any kind.
* `innerHTML` in your own JS.

The first of those is the likely one — a recipe with no instructions is an obvious gap, and
instructions are exactly the field someone wants formatting in.

## B3. What to do when it trips

**First, try not to store HTML at all.** Store markdown or plain text, render it to HTML
server-side with a renderer configured to disable raw HTML passthrough. That sidesteps the whole
problem — most markdown libraries have a "no raw HTML" switch.

If HTML genuinely must be stored:

1. Add **`Ganss.Xss`** (`HtmlSanitizer`) to `FoodCalc.Features`.
2. Sanitise **in the API**, in the create/update command handlers — never in the Blazor client,
   which an attacker can bypass by calling the API directly.
3. Sanitise **explicitly on the specific field**, not via a blanket MediatR pipeline behaviour
   that scrubs every string on every command. A blanket rule will corrupt ordinary text.
4. Use an **allowlist**: `b i em strong p br ul ol li h3 h4 a` and nothing else; on `a`, force
   `rel="noopener noreferrer"` and allow only `http`/`https` hrefs (blocks `javascript:`).
5. Store the sanitised value. Sanitisation is one-way and lossy — that is intended.
6. Keep output encoding everywhere else. Sanitisation is the exception for one field, not a
   replacement for the default.

## B4. The thing that will mislead you

**The review queue gates visibility, not safety.** `IsReviewed` decides who can *see* a recipe; a
moderator clicking approve is not inspecting it for script tags, and cannot be expected to. Do not
treat moderation as a sanitisation step — and note that a recipe is visible to its own author
before review regardless.

## B5. Verification when it trips

* Submit a recipe whose description contains `<script>alert(1)</script>`, `<img src=x onerror=alert(1)>`
  and `<a href="javascript:alert(1)">x</a>`. All three must be inert when rendered.
* Submit the same **directly to the API**, bypassing the web client. Same result.
* An ordinary description with `&`, `<`, `>` and accented characters survives readable.
* With Part A already shipped, a CSP violation should also be logged — defence in depth working.
