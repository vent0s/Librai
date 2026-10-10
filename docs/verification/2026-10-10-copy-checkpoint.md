# Copy repository and query checkpoint — 2026-10-10

The author implemented `CopyRepository`, registered it with scoped DI, and added `GET /copies/byId/{id}` and `GET /copies/byTitle/{id}`. Copy insertion now sets only the Copy entry to Added before saving, without calling `Copies.Add(copy)`. AI reviewed and verified the current checkpoint without editing product code.

## Checks and isolation

- `dotnet build LibrAI.slnx --no-restore` passed with 0 warnings and 0 errors.
- `dotnet ef migrations has-pending-model-changes --project LibrAI.Infrastructure --startup-project LibrAI.Api --no-build -- --environment Development` reported no changes since the last migration.
- A temporary C# probe was compiled with the installed SDK and existing application dependencies, then run with `dotnet exec` using the API runtime/dependency files. It called the actual repositories against PostgreSQL 18.6 in a new container, with a dedicated database/user, random credentials, a random loopback port, and a 256 MiB tmpfs mount. A connection guard rejected the development port.
- Both migrations applied to the isolated database. All inserted titles and copies were synthetic fixtures.
- A separate API process used a random loopback port and a process-local connection override targeting only that isolated database.

## Results

- Missing Copy lookup returned null. Listing an existing title with no copies, or an unknown title, returned an empty list. Null Copy input was rejected.
- A Copy referencing a Title obtained from the existing no-tracking TitleRepository was inserted successfully. The Title remained untracked and the title count did not increase.
- After insertion, the Copy state was Unchanged and a second save wrote no changes. Re-adding the same instance or the same ID in that context returned false.
- A duplicate ID in a fresh context returned false and detached the rejected Copy. A subsequent distinct Copy could be inserted using the same context.
- Two contexts inserting the same Copy ID concurrently produced exactly one true and one false result.
- An invalid TitleId propagated PostgreSQL's foreign-key violation for `FK_Copies_Titles_TitleId`; it was not mistaken for a duplicate Copy.
- Listing by title returned only that title's copies, with Title navigation loaded, and left the fresh context without tracked entities.
- Lookup by Copy ID loaded its Title and tracked the Copy. Calling CheckOut and saving persisted Loaned, verified through a fresh context.
- HTTP lookup returned 200 with the expected Copy and nested Title; a missing Copy returned 404. Listing by title returned the expected filtered array. An unknown title returned `200 []`.
- The API process, isolated container, and tmpfs data were removed. Temporary source, driver, assembly, and compiler-response files were removed after a full workspace reference check. The original development container remained running; no existing database was written to or restarted by these checks.

## Incomplete work

The list endpoint's null branch cannot detect an empty result: ListByTitleIdAsync returns a collection. The author still needs to review route conventions and decide whether an unknown title should be distinguished from an existing title with no copies. This checkpoint records the current `200 []` behavior without changing it.

Copy creation has no HTTP endpoint, and the Copy implementation note is pending. Loan persistence and circulation endpoints, consistent ProblemDetails, structured application logging, and stage-two self-assessment are unfinished. This test did not exercise concurrent borrowing, authorization, or restart persistence for Copy specifically; earlier title restart checks are recorded separately. These are temporary AI-operated checks, not the author-written stage-four test suite.
