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

🚧 Early development — vision and foundation stage. No runnable build yet; the roadmap above is the order of work.

## How it's built

Hand-written and production-grade: core logic is implemented by hand, with AI used for guidance and review rather than code generation. Key decisions are recorded as ADRs (see `docs/`), and delegated work is explicitly logged in [`docs/delegation-log.md`](docs/delegation-log.md).
