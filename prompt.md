# Tic-Tac-Toe Development Prompt Journal

This file captures the kind of prompts and conversations we used while building the project end to end.
It is written in simple language and follows the real path: architecture decisions, logic discussions, realtime updates, testing, and the CSS iterations.

## Prompt Conversation (From Scratch)

### 1) Project Kickoff and Architecture

**Me:** I want to build a full-stack Tic-Tac-Toe app from scratch. I need two modes: two-player and player-vs-computer.

**You:** Good starting point. Let us split responsibilities clearly:
- Backend should own rules, move validation, winner detection, draw detection, undo behavior, and scoreboard updates.
- Frontend should focus on user interaction and rendering the latest state from backend.
- Realtime updates should come from a push channel so state changes appear quickly.

**Me:** What stack should we use?

**You:** Use:
- ASP.NET Core Web API for game endpoints.
- SQLite + EF Core for persistence.
- SignalR for realtime updates.
- Angular standalone app for frontend.

**Me:** Why this architecture?

**You:** Because it keeps business logic centralized, avoids frontend/backend drift, and supports future extension (multiplayer rooms, audit history, analytics).

### 2) Design Pattern Discussion

**Me:** Which design approach are we following?

**You:** Practical layered design:
- Controller layer for HTTP orchestration.
- Rules engine service for game logic.
- Data layer with EF Core context and models.
- DTOs for API contracts.
- Frontend services for API and realtime communication.

**Me:** Why not put logic directly in controller?

**You:** Rules change over time. Keeping logic in a separate engine makes it testable, cleaner, and easier to reason about.

**Me:** What patterns are we applying in frontend?

**You:** Service-oriented UI state flow:
- Game service for REST actions.
- Realtime service for SignalR connection and events.
- Component as presentation and user action handler.

### 3) Requirement to Spec Conversion

**Me:** Convert my requirement into a clean spec before coding.

**You:** Final spec:
1. Start game in selected mode.
2. Make move only on empty cells.
3. Detect win (rows, columns, diagonals).
4. Detect draw when board is full and no winner.
5. In computer mode, computer responds after player move.
6. Undo support:
	- Two-player: remove last move.
	- Computer mode: remove last two moves.
7. Reset game board while keeping scoreboard.
8. Track move history table with move number, player, row, column.
9. Track scoreboard (X wins, O wins, draws).
10. Reset scoreboard separately.
11. Keep frontend state in sync after refresh/reload.
12. Push latest state to clients through SignalR.

### 4) Backend Build Prompts

**Me:** Build backend endpoints for session creation, moves, undo, reset board, read scoreboard, and reset scoreboard.

**You:** We will create and wire:
- Games controller with create/get/move/undo/reset endpoints.
- Scoreboard endpoints.
- Rules engine for board reconstruction and evaluations.
- Hub broadcasting after state changes.

**Me:** How do we model persistent entities?

**You:** Use game session, move log, and scoreboard tables. Store mode, status, winner, current turn, timestamps, and move sequence.

### 5) Logic Discussion (Win, Draw, Computer, Undo, Reset, History, Scoreboard)

**Me:** Explain winner logic clearly.

**You:** Build board from move history and check 8 lines:
- 3 rows
- 3 columns
- 2 diagonals
If all 3 cells in any line have same symbol, set winner and winning cells.

**Me:** Draw logic?

**You:** If no winner and board has 9 moves, set draw.

**Me:** Computer mode logic?

**You:** Priority strategy:
1. Play winning move if available.
2. Block opponent winning move.
3. Take center.
4. Take a corner.
5. Take any remaining cell.

**Me:** Undo logic details?

**You:**
- Two-player: pop one move.
- Against computer: pop two moves so player retries whole turn.
- Do not allow invalid undo transitions.

**Me:** Reset game logic?

**You:** Clear move history for that session, reset status and current turn, keep scoreboard unchanged.

**Me:** Move history table logic?

**You:** Persist every valid move with sequence and coordinates, return sorted list to frontend for display.

**Me:** Scoreboard and reset scoreboard logic?

**You:**
- Increment only when a game reaches terminal status (X win, O win, draw).
- Reset scoreboard endpoint sets all counters to zero.
- Undo after game end should not silently corrupt historical score events.

**Me:** Refresh and reload behavior?

**You:** On load, frontend should hydrate from backend session state immediately and avoid temporary mismatched UI states.

### 6) Why SignalR Discussion

**Me:** Why do we need SignalR here? REST is already present.

**You:** REST handles commands and fetches; SignalR handles push updates. Benefits:
- Immediate state sync after move/undo/reset.
- Better responsiveness than polling.
- Scales to spectators or multi-client game room scenarios.
- Cleaner model: backend emits single source-of-truth updates.

**Me:** Any important setup notes?

**You:**
- Configure CORS for frontend origin and credentials.
- Support transport fallback in client configuration.
- Join per-game groups to broadcast only relevant state.

### 7) Frontend Build Prompts

**Me:** Build Angular pages with landing and game board. Keep mode selection clear and mobile friendly.

**You:** We will include:
- Landing screen with mode selection and start action.
- Game board with 3x3 cells, status panel, controls.
- Move history and scoreboard sections.
- Disabled controls during server actions.

**Me:** Make route behavior stable after refresh.

**You:** We will enforce deterministic startup flow and state hydration to avoid flicker or wrong initial panel.

### 8) CSS Iteration Journey (Real Struggle and Final Direction)

**Me:** First version looks plain. Need better styling.

**You:** Add structure and visual hierarchy first: cards, spacing, board focus, clear action buttons.

**Me:** Still feels off. Colors clash, buttons look generic.

**You:** We refine theme tokens, improve contrast, align typography, and simplify button palette by intent (primary, warning, danger, neutral).

**Me:** Now the board is okay, but result popup timing hides winning cells too fast.

**You:** Delay result overlay slightly so winning pattern remains visible before modal appears.

**Me:** Layout still breaks in some widths.

**You:** Rework responsive breakpoints, tighten grid behavior, and rebalance panel widths.

**Me:** We changed CSS many times and overwrote sections repeatedly.

**You:** Final approach: stabilize baseline styles, then only do small targeted edits to prevent drift.

### 9) Testing and Reliability Prompts

**Me:** Strengthen tests and fix failures before finalizing.

**You:** Actions discussed:
- Expand backend rule tests for board reconstruction and outcomes.
- Fix frontend spec imports and runtime test setup issues.
- Add required testbed providers for current Angular test mode.
- Remove duplicate/obsolete test files causing collisions.

**Me:** Ensure configuration warnings are handled too.

**You:** Update TypeScript config root directory settings to avoid noisy diagnostics and keep tooling stable.

### 10) What Was Suggested vs What Was Manually Changed

This section is for review conversation.

**Suggested during prompting:**
- High-level architecture split.
- Endpoint surface and DTO contract shapes.
- Rules-engine extraction and line-check strategy.
- SignalR connection and broadcast model.
- Test expansion areas and failure triage order.
- CSS direction and iteration checkpoints.

**Changed manually during implementation and review:**
- Final naming, structure, and exact flow in several files.
- UX details around loading, turn indication, and end-state timing.
- Specific CSS values, spacing, colors, and button styles over multiple passes.
- Test expectations and setup adjustments while resolving real failures.

**Reviewed carefully before keeping changes:**
- Winner and draw transitions.
- Computer move safety and priority behavior.
- Undo behavior per mode.
- Scoreboard increment and reset behavior.
- Route reload stability and state hydration.
- Realtime update reliability and fallback behavior.

### 11) Assumptions and Trade-Offs

**Assumptions:**
- Single active game session is acceptable for demo flow.
- Local SQLite persistence is sufficient for evaluation.
- Backend remains authoritative for all game outcomes.

**Trade-offs chosen:**
- Simpler deterministic computer strategy instead of deep minimax search.
- Clear maintainability over premature optimization.
- Fast local setup over distributed production complexity.

### 12) Final Ownership Statement (For Submission)

This submission reflects practical engineering judgment. Prompting helped accelerate drafting and exploration, but final behavior, correctness checks, manual edits, and acceptance decisions were made through direct implementation review and iterative testing.

---

## Reusable Prompt Set (If Rebuilding Again)

1. "Create a full-stack Tic-Tac-Toe app with .NET API, Angular frontend, SQLite persistence, and realtime push updates."
2. "Keep backend as source of truth for game rules, winner/draw checks, and scoreboard state."
3. "Implement two modes: two-player and computer, with clear undo behavior for each mode."
4. "Add move history table and scoreboard endpoints with reset support."
5. "Broadcast latest game state after each command so frontend remains synchronized."
6. "Design responsive UI with clear action hierarchy and visible state transitions."
7. "Refine CSS iteratively based on readability, contrast, and mobile layout behavior."
8. "Expand backend and frontend tests, fix failures, and keep runtime/tooling diagnostics clean."
