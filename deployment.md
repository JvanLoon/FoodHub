# Deployment preparation — code changes

Everything in this document is a change to the **application code and configuration**
needed before FoodHub can be deployed to a server. The server-side procedure lives
in [deploy-steps-hetzner.md](deploy-steps-hetzner.md).

Branch: `feature/hetzner-deploy`.

---

## ⚠️ Before you deploy: rotate every secret

**This repository is public, and these values were committed to it in plain text. They are permanently readable in the
git history by anyone, forever. Scrubbing the working tree does not undo that.** Treat all of them as compromised:

The values themselves are deliberately not reprinted here — read them out of the git history if you need to confirm
which is which.

| Secret                 | Where it leaked                                                                                | Action                                                                          |
|------------------------|------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------|
| Postgres password      | `FoodCalc.Api/appsettings.json`, `appsettings.Development.json`, `docker-compose.yml`          | Generate a new one. Never reuse this value.                                     |
| JWT signing key        | `FoodCalc.Api/appsettings.json` + `FoodCalc.Web/appsettings.json` (both Development files too) | Generate a new one. Anyone holding the old key can mint valid **admin** tokens. |
| pgAdmin password       | `docker-compose.yml`                                                                           | Generate a new one.                                                             |
| Demo account passwords | `IdentitySeed.cs` + the `InitialCreate` migration                                              | Accounts are now deleted (see §1). Never recreate them.                         |

Generate replacements with:

```bash
openssl rand -base64 36
```

For `POSTGRES_PASSWORD`, avoid `:` `@` `/` — they terminate fields in a Npgsql connection string and will produce a
confusing "host not found" error.

The JWT key must be **at least 32 bytes**; the API now refuses to start otherwise. It goes to the **api container
only** — never to `web`. See the warning in §4 for why.

Put the new values in `.env` (gitignored, see `.env.example`). Nothing secret goes back into any committed file.

> Rotating the JWT key invalidates every issued token, so all users are logged out
> once. On a first deployment that is nobody.

---

## 1. No data is seeded any more

This was the main requirement. It is solved in three places, not one.

### 1.1 The seed is gone from the model

`FoodHub.Persistence/Configuration/IdentitySeed.cs` is **deleted**, and the
`IdentitySeed.Seed(modelBuilder)` call is removed from
`FoodHubDbContext.OnModelCreating`.

That file was the root of the problem: `HasData` bakes its contents into the generated migration, so two accounts *and
their password hashes* were committed to a public repo and inserted into every database created from `InitialCreate`.

### 1.2 A migration removes the rows from existing databases

`20260727205147_RemoveSeededIdentityData` deletes the two accounts and their role assignments. It runs automatically at
API startup like every other migration.

Two things were hand-edited after scaffolding, both of which matter:

- **It no longer deletes the three rows from `AspNetRoles`.** The scaffolder wanted to, because the roles left the
  model. But `AspNetUserRoles` has a cascading foreign key to `AspNetRoles`, and real administrators hold those same
  role ids — applying the generated version to an existing database would have silently stripped the Admin role from
  every administrator. The roles now live outside the model and are recreated at runtime instead.
- **`Down()` is a deliberate no-op.** The scaffolder generated an `InsertData` that re-added both published password
  hashes. Rolling back must not restore known credentials.

> Recipes and meal-plan entries reference users through a plain string column with
> **no foreign key**, so anything the demo admin owned on an existing database
> survives as an orphan row. Fresh deployments have nothing to orphan. If you are
> applying this to a database you care about, check first:
> ```sql
> SELECT count(*) FROM "Recipes" WHERE "CreatedByUserId" = 'c2f0b2b0-0000-0000-0000-000000000001';
> ```

### 1.3 A script for databases the migration can't safely handle

`deploy/sql/remove-seeded-identity.sql` does the same job directly in SQL. Use it when the migration is not the right
tool — an environment you would rather clean up by hand, or one where the rows were edited so the migration's
`DeleteData` no longer matches them.

```bash
psql "$CONNECTION_STRING" -v ON_ERROR_STOP=1 -f deploy/sql/remove-seeded-identity.sql
```

It is idempotent, matches on the fixed seed **ids** rather than on email (so a real account reusing
`admin@foodhub.local` is never touched), and **aborts with an error if a seeded account still owns recipes** rather than
orphaning them.

### 1.4 Getting an admin account without seeding one

Deleting the seed leaves a real problem: a fresh database has no roles and no way to log in.
`FoodCalc.Api/Extensions/IdentityBootstrapExtensions.cs` replaces it.

- **Roles** (`Admin`, `Moderator`, `User`) are ensured on every boot. They are named by the authorization policies in
  `Program.cs`, they contain no secret, and creating them is idempotent.
- **The first admin** is created only when *both* `Bootstrap:AdminEmail` and
  `Bootstrap:AdminPassword` are supplied **and** the users table is completely empty. After the first successful boot
  the second condition is false forever, so it cannot re-create or overwrite an account.

A deployment that sets neither variable seeds nothing at all.

```bash
# .env — first boot only
BOOTSTRAP_ADMIN_EMAIL=you@yourdomain.tld
BOOTSTRAP_ADMIN_PASSWORD=<generated>
```

Sign in, change the password in the app, then **blank both lines and recreate the container**. The API logs a warning
naming the account it created, so you can confirm it in `docker compose logs api`.

---

## 2. Secrets moved out of committed files

| File                                                    | Change                                                                       |
|---------------------------------------------------------|------------------------------------------------------------------------------|
| `FoodCalc.Api/appsettings*.json`                        | **Deleted.** All API config now comes from `.env` / environment variables    |
| `FoodCalc.Web/appsettings*.json`                        | **Deleted.** Same treatment                                                  |
| `FoodHub.AppHost/appsettings*.json`                     | **Kept.** Logging levels only, no secrets, and the AppHost is never deployed |
| `docker-compose.yml`, `docker-compose.prod.yml`         | All passwords now `${VAR:?}` — compose fails loudly if unset                 |
| `.env.example` (root, `FoodCalc.Api/`, `FoodCalc.Web/`) | **New.** Templates, placeholders only                                        |
| `.gitignore`                                            | **New entries.** `.env`, `.env.*`, `appsettings.Production.json`             |

Both apps call `DotNetEnv.Env.NoClobber().Load()` at the top of `Main`, guarded by
`File.Exists(".env")` so it is a no-op inside a container.

Two settings did not survive the move, deliberately:

- **`AllowedHosts: "*"`** — that is the framework default when the key is absent, so nothing changed. Still worth
  setting to the real hostname once you have one.
- **`ImportExport:MaxFileSizeInBytes`** — it was already dead. Nothing binds
  `ImportExportSettings`, and both call sites use the `DefaultMaxFileSizeInBytes`
  constant instead. Changing the JSON never did anything. If you want it configurable, that needs a
  `Configure<ImportExportSettings>` registration first.

`${VAR:?message}` is deliberate: an unset variable stops `docker compose` with the message instead of silently starting
Postgres with an empty password.

### There are two different kinds of `.env` here

They are consumed by different things and are not interchangeable:

| File                                       | Read by                                  | Purpose                                                |
|--------------------------------------------|------------------------------------------|--------------------------------------------------------|
| `/.env` (repo root)                        | `docker compose` itself                  | Fills `${POSTGRES_PASSWORD}` etc. in the compose files |
| `/FoodCalc.Api/.env`, `/FoodCalc.Web/.env` | The **DotNetEnv** package at app startup | Local `dotnet run` outside Docker                      |

All three are gitignored. Only the `.env.example` templates are committed.

### Variable naming — the easy mistake

ASP.NET Core's environment-variable provider treats a **double** underscore as the
`:` config separator and nothing else. A single underscore inside a segment is a literal character, so names like
`CONNECTION_STRINGS__DEFAULT_CONNECTION` produce the key `CONNECTION_STRINGS:DEFAULT_CONNECTION` and silently match
nothing — the app then fails at startup with "no connection string is configured", which does not point at the real
cause.

| C# reads                                   | Variable must be                                               |
|--------------------------------------------|----------------------------------------------------------------|
| `GetConnectionString("DefaultConnection")` | `ConnectionStrings__DefaultConnection`                         |
| `Configuration["WebServer:BaseAddress"]`   | `WebServer__BaseAddress`                                       |
| `Configuration["Jwt:Key"]`                 | `Jwt__Key`                                                     |
| `Logging:LogLevel:Microsoft.AspNetCore`    | `Logging__LogLevel__Microsoft.AspNetCore` (the dot is literal) |

`DotNetEnv` is loaded with `NoClobber`, so a real environment variable always beats the file. That is what keeps
Aspire's injected `foodcalc` connection string and the compose-supplied values authoritative.

> Because `FoodCalc.Api/appsettings.json` is gone, `Jwt__Issuer` and `Jwt__Audience`
> no longer have a committed default. The compose files set them explicitly on the
> **api** service — token validation checks the issuer, so a missing value breaks
> every login.

> ⚠️ **`Jwt__Key` goes to the API only — never to the web service.** The tokens are
> HS256, so the key signs as well as verifies: any process holding it can mint a
> token for any account with any role. `web` is the container the tunnel exposes,
> while `api` publishes no ports at all, so giving `web` the key handed the reachable
> process the ability to forge credentials for the protected one. It gained nothing in
> return — the front end never validated a token, it only reads claims to decide what
> to render, and that needs no key.

### Fail-fast validation

`FoodCalc.Api/Program.cs` now rejects a JWT key shorter than 32 bytes at startup. HMAC-SHA256 keys below the hash size
add no security, and this fails on a developer's machine rather than in production.

---

## 3. Reverse-proxy readiness

Behind Caddy the containers only ever speak plain HTTP; TLS terminates at the edge. Without this the app sees scheme
`http` and generates redirects and links on the wrong origin.

Both `Program.cs` files now:

- Configure `ForwardedHeadersOptions` for `X-Forwarded-For` + `X-Forwarded-Proto`, with `KnownIPNetworks`/`KnownProxies`
  cleared.
- Call `UseForwardedHeaders()` instead of `UseHttpsRedirection()` when behind a proxy, controlled by
  `ReverseProxy:Enabled` (defaults to **on** outside Development).

This matters most for the Web project: Blazor Server negotiates its SignalR circuit against an absolute URL, and without
the forwarded scheme it would attempt `ws://`
from an `https://` page and be blocked as mixed content — the app would load and then sit there reconnecting.

> **Clearing `KnownIPNetworks` means trusting whatever sets the forwarded headers.**
> That is only safe because `docker-compose.prod.yml` publishes ports on the Caddy
> container *alone* — `api` and `web` are unreachable from outside the docker network.
> If you ever publish those ports directly, this becomes a header-spoofing hole.

---

## 4. Production stack files

| File                      | Purpose                                                                                    |
|---------------------------|--------------------------------------------------------------------------------------------|
| `docker-compose.prod.yml` | Caddy → web → api → db. Only Caddy publishes ports; no pgAdmin; `restart: unless-stopped`. |
| `deploy/Caddyfile`        | TLS via Let's Encrypt, security headers, WebSocket timeouts disabled for Blazor circuits.  |

The existing `docker-compose.yml` is unchanged in shape and stays the local self-hosted stack.

---

## 5. Local development after these changes

No committed file carries working secrets any more, so a plain (non-Aspire) local run needs a one-time setup:

```bash
cp FoodCalc.Api/.env.example FoodCalc.Api/.env
cp FoodCalc.Web/.env.example FoodCalc.Web/.env
openssl rand -base64 48        # Jwt__Key — FoodCalc.Api/.env only
```

Then fill in the Postgres password in `FoodCalc.Api/.env`.

`FoodCalc.Web/.env` carries no `Jwt__*` at all — see the warning in §4. The web app neither validates tokens nor needs
the key.

Neither project has an `appsettings.json` any more, so **without a `.env` the app does not start** — the API fails on
`WebServer:BaseAddress` or `Jwt:Key` being absent, the web app on `API:BaseAddress`. That is intentional: a missing
config file is a loud failure, an empty committed default is a silent one.

**Running through the Aspire AppHost is unaffected** — it injects its own `foodcalc`
connection string, and `NoClobber` means it wins over the file. Only `Jwt__Key` needs setting.

For `docker compose up`, copy the **root** `.env.example` to `.env` — that is the separate compose-substitution file
described in §2.

For `docker compose up`, copy `.env.example` to `.env` and fill it in.

> **Existing local stack:** `POSTGRES_PASSWORD` is only read when the `pgdata` volume
> is empty. Changing it in `.env` does **not** change the role's password on a volume
> you already have. Either `ALTER USER foodhub WITH PASSWORD '…';` inside the
> container, or `docker compose down -v` and start clean.

---

## 6. Known issues, deliberately not fixed here

These are real, and none of them block a first deployment. Listed so the decision is visible rather than forgotten.

- **`RecipeBlackList.UserId` is a `Guid`** while every other user reference is the Identity string key. The two can
  never match, so the blacklist feature cannot currently work against real user ids. The cleanup script casts around it.
- **`db.Database.Migrate()` runs in-process at API startup.** Fine for one container; it breaks the moment you run a
  second API replica, because both race the same migration. Split it into a one-shot step before scaling out.
- **No retry on the startup migration.** If Postgres is reachable but the database is still being created, the API
  throws and the container restarts. `restart:
  unless-stopped` makes that self-healing but noisy in the logs. `EnableRetryOnFailure`
  on the Npgsql options would be the proper fix.
- **The `AddRecipeFromBlackList` / `RemoveRecipeFromBlackList` handlers ignore their injected `context`** (compiler
  warning CS9113) — they are no-ops.
- **`AllowedHosts` is still `*`.** Setting it to the real hostname is a cheap defence-in-depth win once the domain is
  fixed.

---

## 7. Pre-deploy checklist

- [ ] New Postgres password generated, in `.env`, not reused from git history
- [ ] New JWT key generated (≥32 bytes), identical in the `api` and `web` services
- [ ] New pgAdmin password (or pgAdmin left out of the production stack entirely)
- [ ] `.env` exists on the server and is **not** committed (`git status` is clean)
- [ ] `PUBLIC_HOSTNAME` + `ACME_EMAIL` set
- [ ] `BOOTSTRAP_ADMIN_*` set for the first boot only, and blanked afterwards
- [ ] `docker compose -f docker-compose.prod.yml config` renders with no `?` errors
- [ ] `SELECT "Email" FROM "AspNetUsers";` returns only accounts you created
