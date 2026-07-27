# Deploying FoodHub to Hetzner

Server-side procedure. The code changes it depends on are in
[deployment.md](deployment.md) — do those first.

---

**Target: a Hetzner Cloud server (CX/CPX line) running Ubuntu LTS, with root.**

Hetzner's *Webhosting* line was evaluated and ruled out: it is shared PHP hosting
(hence the PHP Configuration and WordPress entries in konsoleH), with no .NET runtime,
no Docker and no root. FoodHub is a containerised ASP.NET Core app whose front end
holds an open WebSocket per visitor, so it needs a machine it controls. The
PostgreSQL and Redis entries on that plan are managed services for PHP apps to
connect to; they don't change the application tier.

Sizing: 2 vCPU / 4 GB RAM is comfortable for Postgres plus two .NET containers with
build headroom. 40 GB of disk is plenty.

---

## 1. Provision

When creating the server:

1. **Image**: Ubuntu LTS, x86.
2. **SSH keys**: select your key. This puts it on `root`.
3. **Cloud config**: paste [`deploy/cloud-init.yaml`](deploy/cloud-init.yaml), with the
   `ssh_authorized_keys` placeholder replaced by the output of
   `cat ~/.ssh/id_ed25519.pub`. This creates the `foodhub` user, the firewall, and
   Docker on first boot — steps 2 and 3 below are its manual equivalent.
4. **Firewall**: allow inbound **22, 80, 443** only. Deny everything else, in
   particular 5432. (Belt and braces — cloud-init also configures ufw on the host.)
5. Note the IPv4 address.

Cloud-init runs for a minute or two after the server reports ready. Wait for the
marker before doing anything else:

```bash
ssh root@<server-ip> "ls -l /var/log/foodhub-cloud-init-done && cloud-init status"
```

`status: done` means it finished. If it says `error`, read `/var/log/cloud-init-output.log`.

## 2. Verify, then close the root door

**First confirm the unprivileged account works**, because the next command removes
your fallback:

```bash
ssh foodhub@<server-ip> "id && docker run --rm hello-world"
```

That must print `sudo` and `docker` in the group list and run the container. If it
fails, fix it over `ssh root@<server-ip>` — do not continue.

Once it works, turn root SSH off completely:

```bash
ssh foodhub@<server-ip> "sudo sed -i 's/^PermitRootLogin.*/PermitRootLogin no/' /etc/ssh/sshd_config.d/99-foodhub.conf && sudo systemctl restart ssh"
```

**Keep this session open** and confirm in a second terminal that `ssh foodhub@…`
still connects and `ssh root@…` is refused. Getting this wrong locks you out, and
the way back is Hetzner's rescue system.

<details>
<summary>Doing steps 1–2 by hand instead of with cloud-init</summary>

```bash
ssh root@<server-ip>

apt update && apt upgrade -y
apt install -y ufw fail2ban git

adduser --disabled-password --gecos "" foodhub
usermod -aG sudo foodhub
rsync --archive --chown=foodhub:foodhub ~/.ssh /home/foodhub

ufw default deny incoming
ufw default allow outgoing
ufw allow 22/tcp && ufw allow 80/tcp && ufw allow 443/tcp
ufw --force enable

curl -fsSL https://get.docker.com | sh
usermod -aG docker foodhub
```

Then set `PermitRootLogin no` and `PasswordAuthentication no` in
`/etc/ssh/sshd_config`, `systemctl restart ssh`, and verify as above.

</details>

## 3. Confirm Docker

Log in as `foodhub` (a fresh session, so the `docker` group applies):

```bash
ssh foodhub@<server-ip> "docker --version && docker compose version"
```

## 4. DNS

Point an **A record** for your hostname at the server's IPv4 (and `AAAA` at the IPv6
if you're using one). Wait for it to resolve **before** starting the stack — Caddy
requests a certificate on first boot and a failed ACME challenge is rate-limited.

```bash
dig +short yourdomain.tld
```

Do not continue until that prints your server's IP.

## 5. Get the code

```bash
sudo -iu foodhub
git clone https://github.com/JvanLoon/FoodHub.git
cd FoodHub
git checkout feature/hetzner-deploy
```

## 6. Configure secrets

```bash
cp .env.example .env
chmod 600 .env
openssl rand -base64 36   # POSTGRES_PASSWORD  (strip any : @ /)
openssl rand -base64 48   # JWT_KEY
nano .env
```

Fill in `POSTGRES_PASSWORD`, `JWT_KEY`, `PUBLIC_HOSTNAME`, `ACME_EMAIL`, and — **for
this first boot only** — `BOOTSTRAP_ADMIN_EMAIL` and `BOOTSTRAP_ADMIN_PASSWORD`.

Verify everything resolves before building:

```bash
docker compose -f docker-compose.prod.yml config >/dev/null && echo "config OK"
```

Any `is required` error here means a variable is missing from `.env`.

## 7. First boot

```bash
docker compose -f docker-compose.prod.yml up -d --build
docker compose -f docker-compose.prod.yml logs -f
```

The first build pulls the .NET 10 SDK image and takes a few minutes. Watch for, in
order:

1. `db` reports healthy.
2. `api` applies both migrations — `InitialCreate`, then `RemoveSeededIdentityData`.
3. `api` logs `Created bootstrap admin <your email>`.
4. `caddy` obtains a certificate (`certificate obtained successfully`).

`FATAL: database "FoodCalc" does not exist` and `relation "__EFMigrationsHistory"
does not exist` in the Postgres log on a first boot are **normal** — EF probes for the
database, creates it, then reads the history table before creating it.

## 8. Verify

```bash
curl -I https://yourdomain.tld          # expect 200, and a valid certificate
docker compose -f docker-compose.prod.yml ps    # all Up, db healthy
```

Then in a browser:

- [ ] The page loads over HTTPS with no certificate warning.
- [ ] No "Attempting to reconnect" banner — that means the WebSocket is working, and
      it is the single best check that the forwarded-headers config is right.
- [ ] Log in with the bootstrap admin.
- [ ] **`admin@foodhub.local` and `user@foodhub.local` do not exist.** Check the admin
      user list, and confirm directly:

```bash
docker compose -f docker-compose.prod.yml exec db \
  psql -U foodhub -d FoodCalc -c 'SELECT "Email" FROM "AspNetUsers";'
```

Only the account you created should be listed.

- [ ] Add a recipe, then open the calendar and add it to a day through the modal —
      that exercises the query path fixed in `07d2ffc`.

## 9. Close the bootstrap door

Once you have signed in and changed the admin password **in the app**:

```bash
nano .env      # blank BOOTSTRAP_ADMIN_EMAIL and BOOTSTRAP_ADMIN_PASSWORD
docker compose -f docker-compose.prod.yml up -d
```

The bootstrapper is already inert (it refuses to act once any user exists), but
leaving a plaintext admin password in a file on the server has no upside.

---

## 10. Backups

The `pgdata` volume is the only thing that cannot be rebuilt from git. Nightly dump:

```bash
mkdir -p ~/backups
cat > ~/backup-foodhub.sh <<'EOF'
#!/usr/bin/env bash
set -euo pipefail
cd /home/foodhub/FoodHub
STAMP=$(date +%F)
docker compose -f docker-compose.prod.yml exec -T db \
  pg_dump -U foodhub -d FoodCalc --clean --if-exists \
  | gzip > "/home/foodhub/backups/foodcalc-$STAMP.sql.gz"
find /home/foodhub/backups -name 'foodcalc-*.sql.gz' -mtime +14 -delete
EOF
chmod +x ~/backup-foodhub.sh
crontab -e     # 15 3 * * * /home/foodhub/backup-foodhub.sh
```

**A backup you have never restored is not a backup.** Test it once:

```bash
gunzip -c ~/backups/foodcalc-<date>.sql.gz | \
  docker compose -f docker-compose.prod.yml exec -T db psql -U foodhub -d FoodCalc
```

Also copy the dumps off the server — Hetzner's snapshot feature is a separate,
worthwhile layer, but it protects against disk loss, not against you deleting rows.

> Do not commit dumps. This repository is public and the dumps contain real user data.

## 11. Updating

```bash
cd ~/FoodHub
git pull
docker compose -f docker-compose.prod.yml up -d --build
```

Migrations apply on API start. **Take a dump before any deploy that includes a
migration** — `Down()` on `RemoveSeededIdentityData` is intentionally a no-op, so a
rollback is restore-from-backup, not `migrations remove`.

## 12. Reaching the database

Postgres is not published. Tunnel to it rather than opening 5432:

```bash
ssh -L 5433:localhost:5432 foodhub@<server-ip>
```

Then point pgAdmin on your machine at `localhost:5433`. This is why the production
compose file drops the pgAdmin container — a web-exposed DB admin panel is a large
attack surface for something you need a handful of times a year.

## 13. Rollback

```bash
cd ~/FoodHub
git log --oneline -10
git checkout <previous-good-sha>
docker compose -f docker-compose.prod.yml up -d --build
```

If the failed deploy included a migration, restore the pre-deploy dump first, then
check out the older code — in that order.

---

## Open questions for you

1. **What hostname?** Needed for `PUBLIC_HOSTNAME`, the Caddy certificate, and the
   CORS origin. Nothing in §4 onward can be done without it.
2. **Do you want the API reachable from outside?** Right now it isn't — only the web
   container calls it over the internal network. Exposing it means a Caddy route and
   a CORS review.
3. **Email.** Nothing sends mail today, so there is no password reset. Worth deciding
   before real users exist.
