# Agent guidelines

## Git commit standards

```
<type>(<optional scope>): <description>

<optional body>

<optional footer>
```

### Types

| Type | Use for |
| --- | --- |
| `feat` | Add, adjust, or remove a feature (API/UI) |
| `fix` | Fix a bug from a prior `feat` |
| `refactor` | Restructure code without changing behavior |
| `perf` | Performance-focused refactor |
| `style` | Formatting / style only (no behavior change) |
| `test` | Add or correct tests |
| `docs` | Documentation only |
| `build` | Build tools, dependencies, version |
| `ops` | CI/CD, infra, deployment, monitoring |
| `chore` | Maintenance tasks (e.g. `.gitignore`, init) |

### Rules

- Description is mandatory; use imperative present tense (`add`, not `added` / `adds`)
- Do not capitalize the first letter; do not end with a period
- Scope is optional; do not use issue IDs as scopes
- Breaking changes: add `!` before `:` (e.g. `feat(api)!: remove status endpoint`) and document in the footer with `BREAKING CHANGE:`
- Keep commits small and focused; do not commit secrets or local IDE/build artifacts

### Examples

```
feat: add amount-per-uom incentive calculation
fix(rebate): prevent null product from failing silently
test: add rebate service unit tests
refactor: extract incentive calculation strategies
docs: update AGENTS.md with commit conventions
```

## Tests

```bash
dotnet test Smartwyre.DeveloperTest.sln
```
