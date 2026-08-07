# Tic-Tac-Toe Development Prompt Trail

This document captures how the work was actually guided from problem statement to final implementation, using practical prompts, follow-up questions, and technical discussions during coding.

## Problem Statement at Start

Build a full-stack Tic-Tac-Toe application with:
- Two play modes: two-player and player-vs-computer.
- Server-owned rules for move validation and game result.
- Move history and scoreboard persistence.
- Realtime updates so UI stays synchronized.
- Clean, responsive frontend with stable behavior on refresh.

## Prompt and Discussion Timeline

### Prompt 1: Convert requirement to implementation plan

Asked for a clear technical plan before coding:
- How to split backend and frontend responsibilities.
- Which stack to use and why.
- What data model and endpoints are required.

Discussion outcome:
- Backend selected as source of truth for rules and outcomes.
- Angular frontend selected as rendering/action layer.
- SQLite selected for local persistence.
- SignalR selected for push updates after state changes.

### Prompt 2: Define architecture and design patterns

Asked to structure backend and frontend in maintainable layers.

Discussion points:
- Controller layer for HTTP orchestration.
- Rules engine service for game logic.
- EF Core data layer for sessions, moves, and scoreboard.
- DTOs for API contracts.
- Frontend services for REST and realtime.

Why this was chosen:
- Easier testing of game logic.
- Clear separation between behavior and presentation.
- Lower risk of frontend/backend rule drift.

### Prompt 3: Define complete game specification

Asked to finalize exact behavior before implementation:
1. Start game with selected mode.
2. Allow moves only on empty cells.
3. Detect wins for rows, columns, diagonals.
4. Detect draw when board is full without winner.
5. Trigger computer response in computer mode.
6. Support undo by mode.
7. Support reset game without wiping scoreboard.
8. Track move history with sequence and coordinates.
9. Track scoreboard counters for X, O, draw.
10. Support reset scoreboard action.
11. Ensure refresh/reload hydrates current state correctly.
12. Broadcast every meaningful state change.

### Prompt 4: Build backend APIs and models

Asked to implement core endpoints and persistence.

Discussion points:
- Create game session endpoint.
- Move endpoint with validation.
- Undo endpoint with mode-based rules.
- Reset board endpoint.
- Scoreboard read/reset endpoints.
- State mapping response for frontend rendering.

Important alignment done here:
- Terminal states update scoreboard once.
- Move history persisted in order.
- Invalid operations rejected cleanly.

### Prompt 5: Win logic, draw logic, and computer mode logic

Asked for explicit rule reasoning and edge-case handling.

Discussion outcomes:
- Board reconstructed from move history.
- Winner check over 8 lines: 3 rows, 3 columns, 2 diagonals.
- Draw check only when no winner and board full.
- Computer strategy prioritized as:
	1. Finish winning move.
	2. Block opponent winning move.
	3. Take center.
	4. Take corner.
	5. Take remaining cell.

### Prompt 6: Undo, reset, move history, scoreboard logic

Asked to define non-ambiguous behavior for action buttons.

Discussion outcomes:
- Two-player undo removes one move.
- Computer mode undo removes player+computer pair.
- Reset game clears current board state but preserves scoreboard.
- Move history table shows ordered turn details.
- Scoreboard reset is explicit and separate from board reset.

### Prompt 7: Why SignalR is needed here

Asked whether REST alone is enough.

Discussion outcomes:
- REST used for commands and state fetch.
- SignalR used for immediate push synchronization.
- Better UX than polling.
- Better fit for multi-client and spectator-ready behavior.

Implementation considerations discussed:
- CORS configured for frontend origin and credentials.
- SignalR transport fallback enabled for environment compatibility.
- Per-game broadcast grouping for targeted updates.

### Prompt 8: Frontend behavior and reload stability

Asked to eliminate transient wrong states and flicker during startup.

Discussion outcomes:
- Deterministic startup sequence established.
- Immediate hydration from backend session state.
- Busy/disabled action handling improved.
- End-of-game result timing adjusted so winning cells are visible before overlay.

### Prompt 9: CSS direction and iteration struggle

Asked multiple times to improve look and usability.

How the iterations happened:
- First pass improved structure but felt generic.
- Next pass improved contrast and spacing but button hierarchy was weak.
- Another pass tuned board prominence and panel balance.
- Responsive breakpoints were reworked after layout issues.
- Final pass stabilized theme values and avoided broad rewrites.

Final CSS approach:
- Clear visual hierarchy.
- Consistent spacing and rounded card surfaces.
- Distinct action button intent.
- Better mobile behavior and readability.

### Prompt 10: Testing, failures, and cleanup

Asked to make test runs reliable and remove noise.

Discussion outcomes:
- Frontend spec setup issues resolved (imports and test providers).
- Duplicate backend test class collision removed.
- TypeScript config warnings reduced by rootDir alignment.
- Backend and frontend tests aligned to actual behavior.

## Prompt Summary for Review

### What was requested through prompts

- Architecture choices and responsibility split.
- API surface and data contract direction.
- Rule-engine logic and edge-case flow.
- Realtime strategy and connection setup.
- UX refinements for startup, status, and end-game behavior.
- CSS iteration goals and acceptance criteria.
- Test stabilization and diagnostics cleanup.

### What was manually refined during coding

- Exact endpoint behavior and validation details.
- Final interaction timing and UI state transitions.
- CSS values, spacing, and responsive tuning.
- Test expectations and final pass/fail corrections.

### What was reviewed carefully

- Winner/draw correctness.
- Computer strategy behavior.
- Mode-specific undo behavior.
- Scoreboard consistency.
- Realtime synchronization reliability.
- Refresh/reload consistency.

## Clarifications, Assumptions, and Trade-Offs

Clarifications and assumptions:
- 3x3 board only.
- X starts first.
- Backend remains authoritative for outcomes.
- Local SQLite persistence is acceptable for demo scope.

Trade-offs chosen:
- Deterministic computer logic over deep minimax for simplicity.
- Simpler local runtime setup over production deployment complexity.
- Fast iteration on CSS with later stabilization pass.

## Reusable Prompt List

1. Build a full-stack Tic-Tac-Toe app with .NET API, Angular frontend, SQLite persistence, and realtime updates.
2. Keep backend as source of truth for move validation, win/draw logic, and scoreboard updates.
3. Implement two-player and computer modes with clear undo behavior for each mode.
4. Add move history and scoreboard endpoints, including scoreboard reset.
5. Push game state updates after every move/undo/reset so frontend stays synchronized.
6. Improve startup/reload behavior to prevent flicker and temporary incorrect state.
7. Iterate CSS until board readability, action hierarchy, and responsive layout are strong.
8. Expand tests and fix setup failures so both backend and frontend suites are stable.
