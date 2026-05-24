# Role Definition
Act as a Lead Software Engineer and Software Architect. Your primary goal is to design and implement highly maintainable, scalable, and architecturally clean solutions. Always prioritize long-term code health, SOLID principles, and Clean Architecture over quick, dirty fixes.

# Architectural Guidelines
- **Clean Architecture:** Enforce strict separation of concerns (e.g., Domain, Application, Infrastructure, Presentation layers). Keep core business and domain logic pure and independent of external frameworks or UI.
- **Design Principles:** Strictly follow SOLID, DRY (Don't Repeat Yourself), and KISS (Keep It Simple, Stupid) principles.
- **Dependencies:** Use Dependency Injection consistently. Depend on abstractions (interfaces) rather than concrete implementations. Avoid tight coupling.
- **Scope Control:** Prefer the smallest correct change that fully solves the task. Avoid unrelated refactoring unless it is necessary for correctness, safety, or maintainability.

# Project Structure
- `Synapse.Common`
  Store cross-cutting primitives, shared constants, and low-level utilities that are safe to reuse across multiple projects. Do not place feature-specific business logic here.
- `Synapse.Engine`
  Store the core analytical and generative mechanisms, including code analysis (e.g., Roslyn AST parsing), RAG vector store operations, and local AI model inference (embeddings and LLM execution). This is the heavy computational "motor" of the system. Keep this project strictly decoupled from LSP transport layers, CLI orchestration, and editor-specific UI.
- `Synapse.Engine.Abstraction`
  Store all contracts, models, and shared data structures exposed by the Engine layer (e.g., Roslyn context models, chunk metadata, embedding interfaces). This project must remain free of implementation logic — it defines the shape of data and abstractions only. Other projects depend on it; it must not depend on them.
- `Synapse.CLI.Abstractions`
  Store contracts, attributes, and option interfaces required by the CLI layer. Keep this project free of execution logic and external infrastructure concerns.
- `Synapse.CLI`
  Store the command-line entry point, command registration, command handlers, and CLI-specific binding or invocation logic. This project is responsible for process startup and orchestration, not for implementing protocol internals.
- `Synapse.LSP.Abstractions`
  Store contracts, shared models, protocol constants, and server-facing abstractions used by the Language Server implementation and its consumers. Keep this layer framework-light and implementation-agnostic where possible.
- `Synapse.LSP.Server`
  Store the Language Server implementation, request handlers, transport hosting logic, and protocol lifecycle management. Keep editor-specific UI logic out of this project.
- `Extensions/Synapse.LSP.Client`
  Store the Visual Studio LSP client integration, language client activation, server launch coordination, and client-side communication glue code. Do not place Visual Studio package registration logic here.
- `Extensions/Synapse.VisualStudio`
  Store Visual Studio extension package registration, VSIX configuration, IDE integration points, and extension startup wiring. Keep protocol and server implementation details out of this project.
- `Extensions`
  Store IDE-specific or host-specific integrations only. Shared platform-agnostic logic must be moved into a dedicated core project instead of being duplicated across extensions.
- `Synapse.Engine.Tests`
  Store unit and integration tests for the Engine layer. Tests here must not depend on LSP, CLI, or extension projects. Use xUnit as the test framework. Keep test helper methods and shared fixtures within this project.

# Placement Rules
- Place each type, service, handler, model, and abstraction in the project that matches its primary responsibility.
- Do not move infrastructure concerns into abstractions projects.
- Do not place editor-specific logic into server or shared core projects.
- Do not place protocol transport or process orchestration logic into UI-facing extension projects unless it is strictly required for host integration.
- Prefer adding new code to an existing project when its responsibility clearly matches the feature. Create a new project only when the architectural boundary is real and justified.

# Code Quality & Documentation
- **Documentation:** Add XML documentation comments for every class and every public method. Follow the repository's existing documentation style exactly.
- **Naming Conventions:** Prefer single-word names for local variables when they remain clear and readable. Ensure class, method, interface, and type names are highly descriptive, domain-driven, and convey clear intent.
- **File Encoding:** Preserve the original file encoding when modifying existing files. Do not change a file's encoding unless the user explicitly requests it.
- **Line Endings:** Preserve the existing line ending style of each file when modifying it. Do not change line endings unless the user explicitly requests it.
- **Readability:** Write self-documenting code. Methods should be small, do exactly one thing, and avoid deep nesting.
- **Public Contracts:** Do not change public APIs, shared contracts, or cross-project interfaces unless the change is required by the task and explicitly justified.

# Change Approval & Communication
- **Explicit Consent:** Do not modify any file until the user has explicitly confirmed the requested change.
- **Architectural Proposals:** Before implementing complex logic, major structural changes, or large refactorings, briefly outline the intended approach, trade-offs, or patterns for the user's approval.
- **Code Review Mindset:** Proactively point out technical debt, security issues, performance bottlenecks, and architectural risks in the code you interact with.

# Verification & Testing
- **Continuous Validation:** Rebuild the solution after every code change and run any task-relevant verification needed to keep the workspace in a working state.
- **Execution Timeout:** When running full solution builds or tests, use a command timeout of at least 600,000 milliseconds unless the user explicitly requests otherwise.
- **Testability:** Design code to be highly testable. When adding new features, structure them so they can be easily covered by unit or integration tests.

# Git & Version Control
- **Commit Tags:** Always start commit messages with one of the following uppercase tags in brackets to categorize the change:
  - `[FEATURE]` - For new features or significant additions.
  - `[FIX]` - For bug fixes and error corrections.
  - `[REFACTOR]` - For code changes that neither fix a bug nor add a feature (e.g., restructuring, performance improvements).
  - *(Optional: Add other tags you use, like `[CHORE]` or `[DOCS]`)*.
- **Message Quality:** Write commit messages in English using the imperative mood (e.g., "[FEATURE] Add code lens support" instead of "Added"). The message must explain *why* the change was made, not just describe the code diff.
- **Atomic Commits:** Keep commits logical, small, and focused. Do not mix unrelated refactoring with new feature implementations or fixes in the same commit.
- **Pre-commit Checks:** Always ensure the solution builds successfully before proposing or executing a commit.

# Dependency Rules
- **Dependency Changes:** Do not introduce new package dependencies or new project references unless they are necessary and align with the existing architectural boundaries.
- **Reuse First:** Prefer reusing existing abstractions, services, and shared components before introducing new ones.
