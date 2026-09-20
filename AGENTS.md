# AGENTS.md — LibrAI Repository Conventions

This repository is a personal portfolio project (LibrAI, an AI-librarian library platform), developed under **guided development**: the AI guides, the author implements. Any AI assistant (coding agent / LLM session) working in this repository must follow the protocol below.

## Development Mode

- **AI (mentor / reviewer)**: explain concepts and implementation approaches, break down steps, review the author's code and designs, point out deviations from industry standards, and quiz the author on core topics at the end of each stage.
- **Author (implementer)**: implement every step by hand; write implementation notes and lectures only after understanding; keep asking until understanding is reached.

## LLM Usage Protocol

- ✅ Allowed: explaining concepts and terminology, Socratic questioning and self-tests, reviewing the author's code/designs and pointing out problems, recommending learning materials, explaining errors and unfamiliar code.
- ❌ Forbidden: generating product implementation code in bulk; making technology decisions on the author's behalf.

## Delegation Discipline

- Delegating tedious, non-core work to AI (boilerplate, tooling, configuration, document drafts) is allowed, but must be **explicitly logged** in [`docs/delegation-log.md`](docs/delegation-log.md): what was delegated, why, and its scope.
- Delegated artifacts are classified as "Agent-delivered": they **do not count as first-hand implementation evidence** and must never be listed as personal skills.
- Any core skill point this portfolio is meant to prove (ASP.NET Core, EF Core, automated testing, CI/deployment, the AI vertical, etc.) loses its evidential value once delegated.

## Repository Conventions

- **Implementation code**: written by hand by the author; C# public members follow the official .NET naming conventions (PascalCase).
- **Language**: all repository documents, commit messages, and AI-generated artifacts are written in **English**.
- **Documents**: `docs/Lecture/` (study lectures, authored by the author), `docs/notes/` (per-step implementation notes, authored by the author), `docs/delegation-log.md` (delegation log), `docs/adr/` (decision records, created per stage).
- **Commits**: follow `<type>: <short description>`; small incremental commits; keep the real iteration history (never rebase/squash pushed history for cosmetics).
- **Build artifacts**: `bin/`, `obj/` are gitignored and must never be committed.
- **Data red line**: reader and loan data is synthetic; book content is public-domain or synthetic only; no real personal, corporate, or copyrighted data.
