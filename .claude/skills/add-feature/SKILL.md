---
name: add-feature
description: Add a new feature (resource plus endpoints) to the RetroBoard API following the project layout. Use when asked to add a new resource or endpoint group.
---

The feature to add is whatever the user wrote after the command.

1. Create a folder `api/<Feature>/` with one file per public type: the records,
   `I<Feature>Store`, `InMemory<Feature>Store`, and `<Feature>Endpoints`
   with a `Map<Feature>Endpoints` method.
2. Register the store as a singleton and call `Map<Feature>Endpoints` in
   `Program.cs`. Change nothing else in `Program.cs`.
3. Follow the conventions in CLAUDE.md.
4. This task is exempt from the five-file limit in CLAUDE.md.
5. Add integration tests in `api.Tests` for the behavior described.
   Don't modify existing tests.
6. Stop any running API, then run `dotnet build` and `dotnet test`.
   Fix problems until both pass.
7. Commit with the message "Add <Feature>".
8. In your final message, list every decision you made that the request didn't specify.