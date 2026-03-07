# Implementation Conventions

This document captures repo-specific implementation defaults that complement the architecture and engineering contract. Keep it short, concrete, and enforceable in review.

## Design defaults
- Prefer small, testable classes with one clear responsibility.
- Use constructor injection for services and external dependencies.
- Keep orchestration at application boundaries and business logic in core services.
- Depend on abstractions at boundaries when a dependency touches IO, time, environment, or third-party libraries.
- Prefer explicit inputs and outputs over ambient state, static singletons, or hidden side effects.

## CQS and side effects
- Favor command-query separation where it improves clarity and testability.
- Query paths should not perform writes, mutations, or surprising logging-driven behavior.
- Commands should make side effects explicit in their contract and return a result that is easy to assert in tests.

## Boundary rules
- Keep `Renamer.Core` free of CLI and UI concerns.
- Treat CLI and UI projects as composition and delivery layers over core behavior.
- Keep serialization, filesystem access, clock access, and external library integration behind focused interfaces when practical.

## Logging and errors
- Prefer structured logs with stable property names over interpolated-only messages.
- Log at the boundary where context is richest; avoid spreading logging through pure domain logic unless it is operationally important.
- Prefer explicit result models and domain-specific exceptions over vague catch-all behavior.

## Testing defaults
- Add or update tests with behavior changes.
- Favor fast unit tests for core rules and focused integration tests for boundary wiring.
- Design APIs so core logic can be exercised without filesystem, clock, or process-global state.

## What to avoid
- Service location, hidden globals, and static state that make tests order-dependent.
- Mixing orchestration, IO, parsing, and domain decisions in one class.
- Read methods that also mutate state or perform writes unless the behavior is explicit and justified.
- Drive-by refactors outside the active slice or task scope.

## Related docs
- Architecture boundaries: `docs/specs/040-architecture.md`
- Engineering contract and test gates: `docs/specs/070-engineering-contract.md`
- Slice execution workflow: `AGENTS.md`
