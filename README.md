# Tic-Tac-Toe Arena (Full-Stack Real-Time Architecture)

A professional Monorepo-based Tic-Tac-Toe application engineered with **Angular**, **.NET Core Web API**, and **SQLite**, featuring real-time state synchronization powered by **ASP.NET Core SignalR**.

## 🏗️ Architectural Strategy & Design Patterns
*   **State Machine Management:** All evaluation matrices, move calculations, and board conditions are determined strictly on the server side to maintain a single source of truth and prevent client-side data tampering.
*   **Persistent Historical Audit (Undo/Redo):** Moves are logged sequentially in an SQLite table. This provides a clean audit log and allows for precise rollback mechanics.
*   **Real-time Communication:** Built using **SignalR Hubs** instead of raw WebSockets. This architectural choice provides built-in reconnection management, protocol fallbacks, and connection grouping out of the box.
*   **Scoreboard Ruleset (Option A):** Once a match finishes (Victory or Stalemate), the grid locks, points update permanently, and the Undo feature is disabled to preserve match integrity.

## 🤖 GitHub Copilot Development Runbook
This project was systematically generated using a **Dual-Mode System Engine Paradigm**:
1.  **Agent Mode (`@workspace /agent`):** Used to choreograph multi-file structural edits, establish infrastructure pipelines, and build out the SignalR connection matrix across the project directories.
2.  **Ask Mode (`/chat`):** Used to isolate specific business logic, create the database entity configurations, and implement the row/column win-checking algorithms.

## 🚀 Local Run Instructions
### Backend Setup
```bash
cd backend/TicTacToe.Api
dotnet run --urls=http://localhost:5000
```

### Frontend Setup
```bash
cd frontend
npm install
ng serve --port 4200
```
