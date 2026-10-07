# Title creation verification — 2026-10-07

The author implemented the repository and endpoint. AI performed code review and temporary local smoke checks and wrote this record during the requested progress update. This record does not replace the pending author-written implementation note or count as an author-written test suite.

## Checked behavior

The in-memory singleton repository now uses `ConcurrentDictionary<string, Title>`. `TryAddAsync` returns the dictionary's insertion result. `POST /titles` binds a `CreateTitleRequest`, rejects null/empty/whitespace values in Name, ISBN, or Author, creates a server-generated GUID, awaits insertion, and returns 201 with the entity and `/titles/{id}` as Location. All three synthetic seed titles also receive GUIDs. The experimental count-based `GetLastId` method was removed before commit preparation.

## Results

Checks used the local .NET 10 SDK (10.0.302) and temporary API processes listening only on loopback. Requests contained synthetic book data. Processes started for the checks were stopped afterward.

| Check | Observed result |
|---|---|
| `dotnet build --no-restore` after the final source cleanup | Passed; 0 warnings, 0 errors |
| POST a valid title | 201; JSON entity and matching Location |
| GET the returned Location | 200; Id matched the created entity |
| Each required field set to null, empty string, or whitespace, with other fields valid | All 9 cases returned 400 |
| Initial synthetic catalog | 3 entries; all Ids parsed as GUIDs |
| Submit 12 title-creation requests concurrently to a fresh process | All 12 returned 201; 12 distinct Ids |
| Read the catalog after those concurrent requests | 15 entries (3 seeds + 12 new titles); all new Ids present |

The HTTP checks preceded removal of the unused `GetLastId` declaration and implementation; that cleanup did not change the exercised request paths. The final source was rebuilt successfully afterward. An earlier review reproduced a 500 response for a whitespace-only name; the author's switch to `IsNullOrWhiteSpace` corrected it, as confirmed by the final invalid-input checks above.

## Limits and next work

- Concurrency checks created distinct titles with generated Ids. They did not exercise two readers borrowing one copy, or directly force two insertions with the same Id.
- The 500 insertion-failure branch was reviewed but not forced in HTTP testing.
- Validation errors currently return a JSON string; the insertion-failure branch uses ProblemDetails. A consistent error model remains pending.
- Data and seed GUIDs are recreated on restart. Persistence, copy/loan repositories, circulation endpoints, authentication, and a maintained automated test suite remain pending.
- The author will write a concise implementation note before continuing the circulation work.
