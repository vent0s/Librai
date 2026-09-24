# ADR-0001: Use Minimal API as the API style

## Context
LibrAI is a solo-developed service with a handful of resources (books, copies, loans, reservations, the AI librarian) and a low double-digit endpoint count planned across its stages. The first endpoints were written in Minimal API style before the style distinction was fully understood; the decision below was re-made after comparing both styles side by side.

## Decision
Use Minimal API for all endpoints.

## Consequences
Organization is our own job: endpoints are grouped per resource into static extension methods (e.g. `BookEndpoints.Map(app)`); Program.cs stays thin. Migration to controllers, if ever needed, is mechanical (move lambdas into methods, move routes into attributes) — the heavier style stays a deferred option, not a rejected one.

## Alternatives
Controller-based Web API — deferred, not rejected. It offers convention-based organization and mirrors the enterprise codebases of target employers, but its ceremony is not justified at the current scale. Trigger to revisit: when endpoint or resource growth makes extension-method partitioning painful.
