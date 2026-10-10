# Copy model and migration verification — 2026-10-09

The author added `Copy.TitleId`, kept it aligned with `Title.Id` in the public constructor, added a private constructor for EF materialization, registered `DbSet<Copy>`, configured the required one-to-many relationship with restricted deletion, and generated `20261009092529_AddCopies`. AI reviewed and verified these changes without editing product or migration code.

## Scope and isolation

Checks used the built application assemblies and PostgreSQL 18.6 in a dedicated temporary container with its own database/user, a random password, a loopback-only random port, and a 256 MiB tmpfs data mount. The existing development database was inspected only through a read-only transaction: it had `InitialCatalog`, zero titles, and no `Copies` table. It was not migrated or used for write tests.

## Results

- `dotnet build LibrAI.slnx --no-restore` passed with 0 warnings and 0 errors.
- `dotnet ef migrations has-pending-model-changes --project LibrAI.Infrastructure --startup-project LibrAI.Api --no-build -- --environment Development` reported no model changes since the last migration.
- Migration review confirmed `Copies.Id` as primary key, required `TitleId`, integer `Status`, foreign key `FK_Copies_Titles_TitleId` with `Restrict`, and a non-unique index `IX_Copies_TitleId`. `Down` removes only `Copies`.
- A temporary C# probe was compiled with the existing SDK and application dependencies, then run with `dotnet exec` using the API runtime configuration and dependency manifest. Through EF's `IMigrator.MigrateAsync`, it applied `InitialCatalog`, inserted a synthetic title, then applied `AddCopies`. The existing title survived and both migrations were recorded.
- Two copies referencing the same title were saved successfully. A fresh context read them using `AsNoTracking().Include(c => c.Title)`, confirming EF could materialize Copy, preserve its Id/TitleId/status values, load the related Title, and leave no tracked query results.
- `Copy.CheckOut()` followed by `SaveChangesAsync()` persisted the status change; a fresh context read `Loaned`.
- Database metadata confirmed all three Copy columns were non-nullable, the TitleId index was non-unique, and the foreign key used RESTRICT.
- Invalid TitleId insertion was rejected with SQLSTATE `23503`; NULL TitleId with `23502`; duplicate Copy Id with `23505`. Deleting a title referenced by copies was rejected with `23001` (`restrict_violation`). The original title and both copies remained intact.
- The first probe expected `23503` for restricted deletion. PostgreSQL returned its distinct `23001` code, so the temporary test expectation was corrected; the complete rerun passed without any product change.
- Migrating back to `InitialCatalog` removed the Copies table and the AddCopies history entry while preserving the original title and InitialCatalog record. This rollback intentionally discards copy data; it is not a data-restoration mechanism.
- The isolated container and tmpfs data were removed. Temporary source, driver, assembly, and compiler response files were deleted after a repository reference check.

## Boundaries and next step

These are AI-operated checks using synthetic fixtures, not an author-written automated test suite. They verify the Copy entity mapping and migration. Repository, HTTP, and circulation behavior are outside this record; subsequent Copy repository and query-endpoint checks are recorded in the [2026-10-10 checkpoint](2026-10-10-copy-checkpoint.md).

## Development database readback — 2026-10-10

After the author applied `AddCopies`, AI inspected the existing development database through a read-only `psql` transaction. The migration history contained both `20261009073457_InitialCatalog` and `20261009092529_AddCopies`. The Copies columns, primary key, required TitleId foreign key with ON DELETE RESTRICT, and non-unique TitleId index matched the migration. Titles and Copies each contained zero rows. AI did not migrate, write to, or restart the development database during this check.

At this readback, the next guided step was to implement `CopyRepository`. Its subsequent implementation and remaining endpoint work are recorded in the checkpoint linked above.
