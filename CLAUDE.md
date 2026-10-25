# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands
API (run from `api/`):
- `dotnet build`
- `dotnet run` — serves on http://localhost:5073 (`--launch-profile https` adds https://localhost:7262)

Web (run from `web/`):
- `npm install`
- `npm run dev` — Vite dev server
- `npm run build` — type-checks with `tsc` (no emit) then builds with Vite; this is the only type-check/lint step
- `npm run preview`

### API
- One folder per feature: `api/Boards/` holds `Board.cs`, `IBoardStore.cs`,
  `InMemoryBoardStore.cs`, `BoardEndpoints.cs`
- `Program.cs` only wires things up (DI (dependency injection) registration, `Map*Endpoints` calls)
- Endpoints never touch storage directly, only via the store interface
- One public type per file

### Web
- Entry is `index.html` → `src/main.ts`, which renders into `#app` via `innerHTML`.
- TypeScript is strict about unused locals/params and uses `verbatimModuleSyntax` (use `import type` for type-only imports) and `erasableSyntaxOnly` (no enums, namespaces, or parameter properties).

## Conventions
- Nullable reference types on, records for DTOs (data transfer objects)
- Store methods are async and take a `CancellationToken`
- Validation failures return 400 `ProblemDetails`
- Don't add OpenAPI metadata unless asked

## Rules
- No new NuGet or npm packages without asking first
- Keep diffs small: if a task needs more than ~5 files, stop and propose a plan