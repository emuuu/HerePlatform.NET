# Contributing to BlazorHerePlatform

Thank you for considering contributing to BlazorHerePlatform! Every contribution helps make this library better.

## Reporting Issues

- **Bug Reports:** Use the [Bug Report template](https://github.com/emuuu/BlazorHerePlatform/issues/new?template=bug_report.yml) and include reproduction steps, expected/actual behavior, and your environment (.NET version, browser, OS).
- **Feature Requests:** Use the [Feature Request template](https://github.com/emuuu/BlazorHerePlatform/issues/new?template=feature_request.yml) and describe the use case you're trying to solve.

## Development Setup

### Prerequisites

- [.NET 8.0, 9.0, or 10.0 SDK](https://dotnet.microsoft.com/download)
- A [HERE Developer](https://developer.here.com/) account and API key (for running the demo app)

### Clone & Build

```bash
git clone https://github.com/emuuu/BlazorHerePlatform.git
cd BlazorHerePlatform
dotnet restore
dotnet build
```

### Run Tests

```bash
dotnet test tests/HerePlatformComponents.Tests/
```

## Pull Request Guidelines

1. **Branch from `main`** — create a feature or fix branch (e.g. `feat/my-feature` or `fix/issue-42`).
2. **Write tests** — all new functionality should include unit tests.
3. **Fill out the PR template** — describe what changed and why.
4. **Keep PRs focused** — one logical change per PR. Avoid unrelated formatting or refactoring.
5. **Ensure CI passes** — all matrix builds (net8.0, net9.0, net10.0) must be green.

## Code Style

This project uses an `.editorconfig` for consistent formatting. Please ensure your editor respects it:

- 4 spaces indentation (no tabs)
- UTF-8 encoding, LF line endings
- PascalCase for public members, `_camelCase` for private fields
- `System` usings sorted first

## Commit Conventions

We follow [Conventional Commits](https://www.conventionalcommits.org/):

| Prefix | Purpose |
|--------|---------|
| `feat:` | New feature |
| `fix:` | Bug fix |
| `docs:` | Documentation only |
| `test:` | Adding or updating tests |
| `chore:` | Build, CI, dependencies |
| `refactor:` | Code change that neither fixes a bug nor adds a feature |

Example: `feat: add support for 3D building extrusion`

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).
