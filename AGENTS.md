# Agent Workflow Instructions

## Process for every feature/task

1. **Start with a clear task** — understand exactly what needs to be done before coding.

2. **Create a plan first** — analyze the project, create a step-by-step plan. Do NOT modify files until the plan is reviewed and approved.

3. **Work in small batches** — execute one step at a time. Do not proceed to the next step until the current one builds successfully.

4. **Always verify** — run `dotnet build` after every change. Fix all compilation errors before moving on.

5. **Explain every file modified** — for each file: why it changed, what changed, any risks introduced.

6. **Use Git constantly** — commit a baseline before major work. Review `git diff` and `git status` when done.

7. **Good workflow for .NET projects**:
   - Step 1: Analyze current architecture, create plan, do not edit files yet
   - Step 2: Execute Step 1 only, build, fix errors
   - Step 3: Explain all modifications
   - Step 4: Execute Step 2 only, build, fix errors
   - Repeat until complete

8. **Prompt template**: "Implement <feature>. Requirements: Follow existing project patterns, Keep API contracts unchanged, Explain every file modified, Run dotnet build, Fix compilation errors, Do not proceed to next step until build succeeds."

## EF Core Migration — Existing Database

When migrating an existing database to EF Core:

1. **Generate the migration**: `dotnet ef migrations add <Name>`
2. **Generate the SQL script**: `dotnet ef migrations script -o script.sql`
3. **Before applying**: Show the user:
   - Migration name
   - Confirm no DROP/ALTER operations
   - Show the exact SQL
   - Explain table-to-model mappings
4. **Option B (seed history)**: Only insert into `__EFMigrationsHistory` to mark migration as applied — no CREATE/ALTER/DROP on existing tables:
   ```sql
   INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
   VALUES (N'<MigrationId>', N'<Version>');
   ```
5. Do not apply blindly — get explicit approval first.
