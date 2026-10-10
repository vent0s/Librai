# LibrAI

> A library platform with Sibyl — an AI librarian who knows exactly what's on the shelves.

**LibrAI** is an AI-librarian-powered library platform. Beyond complete circulation management (cataloging, borrowing, returns, renewals, overdue handling, and reservations), its heart is **Sibyl**, an AI librarian who chats with visitors to understand what they need — and recommends **only books that actually exist in the catalog**.

## Why this exists

Most "AI recommendation" demos are demos. LibrAI is built as a small-but-real production system where the AI is held to the same standard as the rest of the software:

- **Grounded or silent.** Sibyl recommends only from the catalog, and answers questions about book content only with cited passages. When the archives hold no record, she says so — in character: *"The archives hold no record of this."*
- **Measured, not vibes.** Recommendation quality is tracked against a fixed evaluation set (top-k hit rate); the eval suite lives in this repo and can be re-run with one command.
- **Fails like software should.** When the LLM provider is unreachable, the service degrades gracefully to a rule-based fallback instead of falling over — and recovers on its own.
- **Correct under pressure.** Two readers borrowing the same copy at the same moment resolve to exactly one winner. Overdue-notification jobs survive crashes without double-sending. Retried requests never double-issue anything.

## Sibyl, the librarian

Sibyl is an original character — a calm, book-loving sage. Her persona is not decoration: the product's honesty rules are expressed through her voice, which makes grounded AI behavior part of the experience rather than an error message.

## Product scope

- Catalog with titles and physical copies, readers, and loans
- Borrow / return / renew lifecycle with reservation queues and overdue background jobs
- Role-based access (reader / librarian / admin), enforced server-side
- Sibyl: conversational recommendation + a grounded reading companion over public-domain books
- Reader-facing and admin web UI

## Tech vision

Modern **.NET (LTS)** with **ASP.NET Core** · **PostgreSQL** via **EF Core** · authentication & role-based authorization · **xUnit** unit and integration tests (**Testcontainers**) · **Docker** multi-stage builds · **CI** pipeline · cloud deployment · **React / TypeScript** front end · LLM integration with an evaluation suite, graceful degradation, and cost/latency tracking.

## Roadmap

Circulation core → concurrency & reservations → automated testing → AI librarian vertical (with evals) → web UI & deployment.

## Status

As of 2026-10-10, stage two is in progress. The .NET 10 API uses EF Core/Npgsql repositories and PostgreSQL for titles and copies; the in-memory title repository has been removed. `InitialCatalog` and `AddCopies` create the required tables and relationship. Title creation and lookup persist across restarts, and implementation notes [04](docs/notes/04-title-creation.md) and [05](docs/notes/05-postgresql-persistence.md) are recorded.

The current Copy endpoints are `GET /copies/byId/{id}` and `GET /copies/byTitle/{id}`. Copy insertion is implemented in the repository but has no HTTP creation endpoint yet. Repository and query-endpoint checks passed in an isolated database; see [checkpoint verification](docs/verification/2026-10-10-copy-checkpoint.md), [title persistence checks](docs/verification/2026-10-09-postgresql-bootstrap.md), and [Copy migration checks](docs/verification/2026-10-09-copy-migration.md).

This is an unfinished checkpoint. The copy-list endpoint returns `200 []` for an unknown title; its `copies == null` branch cannot handle an empty list. Route conventions and missing-title behavior still need review, followed by Copy creation and its implementation note. Loan persistence, borrowing/return/renewal endpoints, consistent ProblemDetails, structured application logging, stage-two self-assessment, and all later authentication/test-suite/AI/UI work remain unfinished. Temporary AI-operated checks are not an author-written automated test suite.

## Local database on Windows

Start Docker Desktop with its Linux engine. Create `.env` beside `compose.yaml` with `LIBRAI_DB_PASSWORD` set to a local development password; `.env` is Git-ignored.

- Double-click [`start-db.bat`](start-db.bat) to start the `librai-dev` database service and wait for PostgreSQL readiness at `127.0.0.1:15432`.
- Double-click [`stop-db.bat`](stop-db.bat) to stop that service while retaining its container and named data volume.
- In a terminal or automation, use `start-db.bat --no-pause` or `stop-db.bat --no-pause` to return immediately after completion with an exit code. Both scripts locate the repository from their own directory.

These helpers target Docker Desktop's `desktop-linux` context and the `db` service only. The stop helper can run without `.env`; it supplies a process-local placeholder solely to satisfy Compose interpolation and never changes the database password. Readiness checks do not replace authentication or application migration checks.

For a new local development setup, configure `ConnectionStrings:Library` in API User Secrets. Use host `127.0.0.1`, port `15432`, database/user `librai`, and the same password as the database service. The `.env` file configures Compose; it does not configure the API connection string. Replace the placeholder below with your local password:

```powershell
dotnet user-secrets set "ConnectionStrings:Library" "Host=127.0.0.1;Port=15432;Database=librai;Username=librai;Password=<local-password>" --project LibrAI.Api
dotnet tool restore
dotnet ef database update --project LibrAI.Infrastructure --startup-project LibrAI.Api -- --environment Development
dotnet run --project LibrAI.Api --launch-profile http
```

Use the listening URL printed by the API. A new database starts empty; there is no automatic in-memory seed. Stopping the database with the helper preserves its data volume.

## How it's built

Hand-written and production-grade: core logic is implemented by hand, with AI used for guidance and review rather than code generation. Key decisions are recorded as ADRs (see `docs/`), and delegated work is explicitly logged in [`docs/delegation-log.md`](docs/delegation-log.md).
