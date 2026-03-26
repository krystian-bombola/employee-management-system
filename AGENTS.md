# AGENTS.md

## Scope
- These instructions apply to the entire repository.

## Project Overview
- This repository contains a desktop employee management application built with Avalonia UI and .NET.
- The main application project lives in `employee-management-system/`.
- The solution entry point is `employee-management-system.sln`.

## Tech Stack
- UI: Avalonia `11.3.12`
- Runtime: `.NET 10`
- Architecture: MVVM with `CommunityToolkit.Mvvm`
- Data access: Entity Framework Core with SQLite

## Working Guidelines
- Keep changes focused and minimal; avoid broad refactors unless explicitly requested.
- Preserve the existing MVVM structure across `Models`, `ViewModels`, `Views`, `Services`, `Repositories`, and `Data`.
- Match the existing naming and code style used in nearby files.
- Update documentation when behavior, setup, or developer workflow changes.

## Build And Validation
- Restore/build from the repository root with `dotnet restore employee-management-system.sln` and `dotnet build employee-management-system.sln`.
- If a change is localized, prefer validating the smallest relevant surface first.
- Do not fix unrelated build or environment issues unless the user asks.

## File Hygiene
- Keep generated files out of version control.
- Respect the existing `.gitignore` entries for `bin/`, `obj/`, `packages/`, `.vs`, and JetBrains cache folders.

## Notes
- Repository documentation is partly in Polish; preserve the existing language of any file you edit unless the user asks otherwise.
