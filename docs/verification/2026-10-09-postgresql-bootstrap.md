# PostgreSQL bootstrap verification — 2026-10-09

The author created Infrastructure, `LibraryDbContext`, its API DI registration, and the Compose configuration under guided development, corrected `Title.Isbn` after model review, and started Docker Desktop. AI performed the build/model checks, Compose startup, and connectivity checks below and wrote this record. These checks are not an author-written automated test suite.

## Results

| Check | Observed result |
|---|---|
| `dotnet build LibrAI.slnx --no-restore` | Passed after the DI registration; 0 warnings, 0 errors |
| Temporary EF model initialization and SQL generation | Passed after renaming the entity property from `ISBN` to `Isbn`; `Titles` has primary key `Id` |
| `docker compose -f compose.yaml config --quiet` | Passed with the author's local `.env`; the file is Git-ignored and untracked |
| Resource preflight | No matching `librai-dev` containers, volumes, or networks; host port 15432 had no listener |
| `docker compose -f compose.yaml up -d db` | Created the dedicated network, named volume, and `librai-dev-db-1`; pulled `postgres:18.6-bookworm` |
| `docker compose -f compose.yaml exec -T db pg_isready -U librai -d librai` | Accepting connections |
| Container inspection | Published only on `127.0.0.1:15432`; `librai-dev_pgdata` mounted at `/var/lib/postgresql` |
| Temporary host-side Npgsql connection with the configured password | Authentication succeeded; database/user both `librai`; server reported PostgreSQL 18.6 |
| `LibraryDbContext.Database.CanConnectAsync()` in the temporary probe | Passed with explicitly supplied probe connection options |
| Read-only schema inspection | Zero public base tables; no application migrations or data had been written |

The temporary C# probes used the existing .NET 10.0.201 SDK and built application dependencies. Their source and assemblies were removed after repository reference checks. Password values were not printed or recorded. The new development container remains running for the next exercise.

## Windows database helper verification

The author requested root-level startup/shutdown batch files. AI implemented `start-db.bat` and `stop-db.bat` and recorded the delegation separately.

- `start-db.bat --no-pause` returned 0 while the database was already running and reported PostgreSQL readiness.
- From the parent Career directory, invoking the absolute path to `stop-db.bat --no-pause` returned 0. Container inspection confirmed `exited`, and the named volume still existed.
- Invoking the absolute path to `start-db.bat --no-pause` returned 0 and waited for readiness. The container ID, volume creation time, and container environment matched their pre-test values; no password was printed.
- The final state was running on `127.0.0.1:15432`. Both batch files use CRLF line endings. `git diff --check` passed.

Automated runs exercised `--no-pause`; interactive double-click behavior, missing-Docker errors, and stopping without `.env` were not exercised. The scripts operate only the `db` service in `librai-dev` on the `desktop-linux` context. They do not remove volumes or stop Docker Desktop.

## API User Secrets verification

The author initialized User Secrets for `LibrAI.Api` and saved `ConnectionStrings:Library`. After these changes, `dotnet build LibrAI.slnx --no-restore` passed with 0 warnings and 0 errors.

A temporary probe loaded the API assembly's User Secrets and constructed a Development `WebApplication` builder with the API application name and content root. The effective connection string matched the saved secret. Before connecting, the probe verified the target was `127.0.0.1:15432`, database/user `librai`. Resolving `LibraryDbContext` through the same DI registration pattern and calling `Database.CanConnectAsync()` returned `true`; a read-only schema query still found zero public base tables. The probe did not start an HTTP listener, run migrations, or print credentials. Its temporary source, assembly, and compiler response file were removed after a repository reference check.

## Initial migration verification

The author installed local `dotnet-ef` 10.0.4, added `Microsoft.EntityFrameworkCore.Design` 10.0.4 to the API with `PrivateAssets=all`, and generated `20261009073457_InitialCatalog` in Infrastructure. AI reviewed and verified the generated files without modifying them.

- `dotnet ef --version` reported 10.0.4. `dotnet build LibrAI.slnx --no-restore` passed with 0 warnings and 0 errors.
- `dotnet ef migrations has-pending-model-changes --project LibrAI.Infrastructure --startup-project LibrAI.Api --no-build -- --environment Development` reported no model changes since the migration.
- `dotnet ef migrations script 0 InitialCatalog` and `dotnet ef migrations script InitialCatalog 0`, with the same project/startup/environment options, generated forward and rollback SQL successfully.
- Both scripts were executed using `psql -X -v ON_ERROR_STOP=1` in a separate PostgreSQL 18.6 container. It used `--network none`, no published ports, and a 256 MiB tmpfs data mount. The existing development container and volume were not used for migration tests.
- Forward SQL created `Titles` and recorded the migration in `__EFMigrationsHistory`. Assertions confirmed six `text` columns, rejection of duplicate primary keys, rejection of NULL for `Id`, `Name`, `Isbn`, and `Author`, and acceptance of NULL for `Description` and `Publisher`.
- Rollback SQL removed `Titles` and the migration-history entry. The history table itself remained, as expected.
- The isolated container and its tmpfs data were removed. Temporary SQL files were deleted after a repository reference check. A final read-only query against the development database still found zero public base tables, and the original development container remained running.

These are AI-operated checks of generated migration SQL, not an author-written automated test suite or an AI application of migrations to the development database.

## Development migration application

After the author reported applying `InitialCatalog` with the instructed EF CLI command, AI inspected the development database using `psql -X -v ON_ERROR_STOP=1` inside an explicit read-only transaction. `__EFMigrationsHistory` contained `20261009073457_InitialCatalog` with product version 10.0.4. `Titles` had the expected six columns and nullability, primary key `Id`, and zero rows. The existing development container remained running on `127.0.0.1:15432`. AI performed no database writes or service restart for this check; HTTP persistence remains to be exercised.

## Database-backed title repository and HTTP verification

The author implemented `TitleRepository`, corrected the primary-key constraint name and same-context duplicate handling, registered `ITitleRepository` with `AddScoped`, and deleted `InMemoryTitleRepository`. AI reviewed these changes and ran the checks below without editing product code.

- `dotnet build LibrAI.slnx --no-restore` passed with 0 warnings and 0 errors. No application-code references to the deleted repository remained.
- A dedicated PostgreSQL 18.6 container used a newly created disposable named volume, database/user `librai_verify`, a random password, and a random loopback-only host port. No existing database or service was used as the write-test target.
- `dotnet ef database update InitialCatalog --project LibrAI.Infrastructure --startup-project LibrAI.Api --no-build -- --environment Development` applied the migration to that isolated database using a process-local connection-string override. Initial history lookup logged a missing-table command failure on the empty database; migration creation/application then completed successfully with exit code 0.
- A temporary C# probe, compiled against the built application assemblies and executed with `dotnet exec`, exercised the actual repository. Empty lists, missing IDs, null-input rejection, successful insertion, and fresh-context materialization passed. Read-only queries left no tracked entities.
- Re-adding the same object and adding another object with the same Id in one context returned `false`. A duplicate in a fresh context also returned `false`, detached the failed entity, preserved the original row, and allowed a subsequent valid insertion in that context.
- Concurrent insertions of the same Id through two separate contexts produced exactly one `true` and one `false`. A temporary check constraint in the isolated database confirmed that a different database error propagated as `DbUpdateException`; that test constraint was then removed.
- The probe launched the actual API on isolated loopback ports with the test connection override. `POST /titles` returned 201, a generated GUID, and the correct Location. List/get endpoints read persisted data, including Unicode and nullable fields. Missing IDs returned 404; blank required fields, null input, and malformed JSON returned 400. Parallel valid POST requests succeeded with distinct IDs. A direct database query confirmed only the expected six synthetic rows existed.
- The HTTP-created title survived an API process restart and a PostgreSQL container restart using the same dedicated volume. The first restart attempt exposed a test-harness issue: Docker reassigned the random published port. The probe was corrected to inspect the new port before restarting the API, and the complete run then passed. No product change was needed.
- All probe-owned API processes, containers, and volumes were removed. Temporary source, driver, assembly, and compiler response files were deleted after a repository reference check. Final read-only inspection found the development database still contained zero titles and the expected migration record; its original container remained running.

These checks use synthetic data and are AI-operated verification, not an author-written automated test suite. Container restart persistence was tested; host reboot, backup/restore, prolonged load, and full deployment were not exercised.

## Boundaries and next step

The existing title endpoints now resolve the scoped EF Core repository, and the in-memory implementation has been removed. The development connection and applied migration are verified, and repository/HTTP behavior has passed isolated integration checks. The author's concise implementation note, `docs/notes/05-postgresql-persistence.md`, passed review after correcting the DbContext/repository relationship and DI registration wording. Subsequent work is recorded in [Copy migration verification](2026-10-09-copy-migration.md) and the [Copy repository/query checkpoint](2026-10-10-copy-checkpoint.md); Loan persistence remains unimplemented.

Application persistence across API and database-container restarts passed in the isolated environment. AI verification did not write to or restart the existing development database or unrelated services; the author applied the development migration.
