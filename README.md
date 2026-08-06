# Tic-Tac-Toe Arena

A full-stack Tic Tac Toe application built for the ABB interview problem statement. It uses an Angular frontend and a .NET Web API backend, with the backend acting as the source of truth for gameplay, move history, scoreboard, and outcome validation.

## Features implemented
- Two-player gameplay
- Basic computer opponent mode
- Move history with row/column positions
- Undo support for the selected mode
- Scoreboard tracking for X wins, O wins, and draws
- Reset game and reset scoreboard actions
- Server-side win, draw, and computer-move rules
- Simple SignalR-based state updates for the UI

## Tech stack
- Frontend: Angular + TypeScript
- Backend: ASP.NET Core Web API
- Storage: SQLite via Entity Framework Core
- Tests: xUnit for backend rules

## Run locally
### Backend
```bash
cd backend/TicTacToe.Api
dotnet run --urls=http://localhost:5000
```

### Frontend
```bash
cd frontend
npm install
npm start
```

The Angular app expects the API at http://localhost:5000.

## API summary
- POST /api/games – create a new game session
- GET /api/games/{id} – fetch current state
- POST /api/games/{id}/moves – submit a move using a body such as { "cellIndex": 4 }
- POST /api/games/{id}/undo – undo the last move
- POST /api/games/{id}/reset – reset the current board
- GET /api/scoreboard – read scoreboard values
- POST /api/scoreboard/reset – reset scoreboard values

## Tests
```bash
cd backend
dotnet test TicTacToe.Tests/TicTacToe.Tests.csproj
```

## AI workflow notes
- The solution was built with a structured prompt-driven workflow: first the problem statement and requirements, then the backend rules and persistence layer, then the frontend UI and API integration.
- The core rules were reviewed and tested explicitly to keep the explanation simple and robust.

## Design decisions
- Backend owns the game rules and state to keep the experience consistent and auditable.
- Scoreboard behavior follows Option A: once a game is completed, undo is disabled and the scoreboard remains final for that game.
- The computer opponent uses a lightweight rule-based strategy: win, block, center, corner, then fallback.

## Assumptions and limitations
- The solution uses in-memory-style backend persistence through SQLite for local demo purposes.
- The computer mode is intentionally simple and deterministic.
- There is no authentication or multi-user persistence beyond the local demo setup.
