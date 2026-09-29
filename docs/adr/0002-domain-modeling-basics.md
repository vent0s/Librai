# ADR-0002: Domain modeling basics — entity shape, IDs, Reader, transition failure style

## Context
Stage 2's first hammer produced the three entities (Title, Copy, Loan). Four follow-up modeling decisions were made by the author in conversation (2026-09-28 / 09-29) and are recorded together as one micro-ADR per the slimmed-down process.

## Decisions
1. **Entity shape** — classes with private setters. State changes only through methods on the entity that owns the state (`Copy.CheckOut()`), never through assignable properties. All legal changes to a copy's or loan's state are visible in that entity's file alone.
2. **IDs** — `string`, tentative. Constructors guard null/whitespace only; a real generation strategy is deferred.
3. **Reader** — no Reader entity in stage 2. `Loan` carries `BorrowerId` as a plain field; a Reader aggregate appears only if/when stage-3 auth needs reader accounts.
4. **Illegal transitions** — entity methods throw `InvalidOperationException`. Exceptions are the service's internal language: endpoints later translate them into 409 responses, and the stage-5 agent tool layer translates them into JSON results. Ruling as stated by the author: "before JSON returns exist, throw." Revisit exceptions-vs-result-objects when the tool layer lands.

## Consequences
Rules live next to the data they protect and cannot be bypassed by callers. Translation happens at boundaries only, so the core never changes shape for a new consumer (C# callers today, HTTP later, agent tools in stage 5). The exception style keeps stage-2 code minimal and defers the "expected business rejection vs. programmer error" split to the boundary that actually needs it.

## Alternatives
Records or public setters with external validation — rejected: rules scatter away from the data they protect. Result objects / Try-pattern for transition failure — deferred, not rejected; explicit trigger is the stage-5 tool layer. A separate LoanService for orchestration — deferred; triggers are a second consumer of the same steps or an endpoint handler growing hard to read.
