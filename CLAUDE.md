# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands
API (run from `api/`):
- `dotnet build`
- `dotnet run` — serves on http://localhost:5073 (`--launch-profile https` adds https://localhost:7262)
- Stop `dotnet run` before `dotnet build`, it locks the output

API tests (run from `api.Tests/`):
- `dotnet test` — xUnit v3 on Microsoft.Testing.Platform (opted in via root `global.json`); hosts the API in-process with `WebApplicationFactory<Program>`
- API tests: `cd api.Tests && dotnet test`
- Never edit existing tests to make them pass. If a test looks wrong, say so and stop.

Web (run from `web/`):
- `npm install`
- `npm run dev` — Vite dev server
- `npm run build` — type-checks with `tsc` (no emit) then builds with Vite; this is the only type-check/lint step
- `npm run preview`

## API
- One folder per feature: `api/Boards/` holds all the files pertaining to that feature. Other features should follow this same pattern.
- One public type per file
- `Program.cs` only wires things up (DI (dependency injection) registration, `Map*Endpoints` calls)
- Endpoints never touch storage directly, only via the store interface

## Web
- TypeScript is strict about unused locals/params and uses `verbatimModuleSyntax` (use `import type` for type-only imports) and `erasableSyntaxOnly` (no enums, namespaces, or parameter properties).

## Conventions
- Nullable reference types on, records for DTOs (data transfer objects)
- Store methods are async and take a `CancellationToken`
- Validation failures return 400 `ProblemDetails`
- Don't add OpenAPI metadata unless asked
- File-scoped namespaces, `RetroBoard.Api.<Feature>`

## Rules
- No new NuGet or npm packages without asking first
- Keep diffs small: if a task needs more than ~5 files, stop and propose a plan