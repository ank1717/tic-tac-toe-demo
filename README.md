# Tic-Tac-Toe Arena

A full-stack Tic Tac Toe application built for the ABB interview problem statement. It uses an Angular frontend and a .NET Web API backend, with the backend acting as the source of truth for gameplay, move history, scoreboard, and outcome validation.

## Features

- Two-player and vs-computer gameplay modes
- Move history with row/column positions
- Undo support per game mode
- Scoreboard tracking (X wins, O wins, draws) — backend-served
- Reset game and reset scoreboard actions
- Server-side win, draw, and computer-move logic
- Real-time state updates via SignalR

## Tech Stack

| Layer     | Technology                          |
|-----------|-------------------------------------|
| Frontend  | Angular 20 + TypeScript             |
| Backend   | ASP.NET Core 10 Web API             |
| Database  | SQLite via Entity Framework Core    |
| Realtime  | SignalR                             |
| Tests     | xUnit                               |

---

## Prerequisites & Setup

Follow these steps **in order**. Both the backend and frontend must be running at the same time.

---

### Step 1 — Install .NET 10 SDK (Backend requirement)

Check if you already have it:

```bash
dotnet --version
```

If the output starts with `10.`, you are good. Otherwise, download and install it from the official page:

- **Windows / macOS / Linux:** https://dotnet.microsoft.com/en-us/download/dotnet/10.0

After installation, verify:

```bash
dotnet --version
# Expected output: 10.x.x
```

---

### Step 2 — Install Backend NuGet packages

Navigate to the API project folder and install the required database packages:

```bash
cd backend/TicTacToe.Api
```

```bash
# SQLite database provider for .NET 10
dotnet add package Microsoft.EntityFrameworkCore.Sqlite

# EF Core design tooling (used for migrations)
dotnet add package Microsoft.EntityFrameworkCore.Design
```

---

### Step 3 — Build and run the Backend

Run these commands from `backend/TicTacToe.Api`:

```bash
# Clean any previous build artifacts
dotnet clean

# Compile the project
dotnet build

# Start the API server on port 5000
dotnet run --urls=http://localhost:5000
```

You should see output like:

```
Now listening on: http://localhost:5000
```

> **Keep this terminal open.** The frontend depends on this server.

---

### Step 4 — Install Node.js (Frontend requirement)

Check your current Node version:

```bash
node --version
```

The app requires **Node 22.x.x or higher**. If you need to install or upgrade:

- **Download Node 22 LTS:** https://nodejs.org/en/download

To install a specific version using `nvm` (recommended):

```bash
# Install nvm if you don't have it: https://github.com/nvm-sh/nvm
nvm install 22
nvm use 22
node --version
# Expected output: v22.x.x
```

---

### Step 5 — Install Frontend dependencies and run

Open a **new terminal window** and run:

```bash
cd frontend

# Install all npm packages
npm install

# Start the Angular dev server
npm start
```

The app will be available at: **http://localhost:4200**

---

### Troubleshooting — Clear Angular Cache

If the frontend behaves unexpectedly after a code change, clear the Angular build cache:

```bash
cd frontend
rm -rf .angular/cache
npm start
```

---

## Running Both Servers (Summary)

| Terminal | Command | URL |
|----------|---------|-----|
| Terminal 1 (Backend) | `cd backend/TicTacToe.Api && dotnet run --urls=http://localhost:5000` | http://localhost:5000 |
| Terminal 2 (Frontend) | `cd frontend && npm start` | http://localhost:4200 |

Open your browser at **http://localhost:4200** to play.

---

## API Reference

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/games` | Create a new game session |
| `GET`  | `/api/games/{id}` | Get current game state |
| `POST` | `/api/games/{id}/moves` | Submit a move — body: `{ "cellIndex": 4 }` |
| `POST` | `/api/games/{id}/undo` | Undo the last move |
| `POST` | `/api/games/{id}/reset` | Reset the current board |
| `GET`  | `/api/scoreboard` | Read scoreboard |
| `POST` | `/api/scoreboard/reset` | Reset scoreboard |

---

## Running Tests

### Backend — xUnit (terminal output)

```bash
cd backend
dotnet test TicTacToe.Tests/TicTacToe.Tests.csproj
```

Results are printed directly in the terminal. There is no separate browser UI for xUnit — pass/fail output looks like:

```
Passed!  - Failed: 0, Passed: 6, Skipped: 0, Total: 6
```

To generate an HTML report you can open in a browser:

```bash
dotnet test TicTacToe.Tests/TicTacToe.Tests.csproj \
  --logger "html;logfilename=TestResults.html"
```

The report is saved to `backend/TicTacToe.Tests/TestResults/TestResults.html`. Open it in any browser.

---

### Frontend — Jasmine + Karma (browser UI)

```bash
cd frontend
npm test
```

Karma will compile the specs and **automatically open a browser window** at:

```
http://localhost:9876
```

That page shows the Jasmine test runner with a live list of every spec — green for passing, red for failing. The results update in real time as you edit the code.

> If the browser does not open automatically, navigate to `http://localhost:9876` manually after the terminal shows `Executed X of X SUCCESS`.

---

## Design Decisions

- **Backend owns the rules.** All win/draw/computer-move logic lives in the API to keep the frontend purely presentational.
- **Scoreboard is final once a game ends.** Undo is disabled after a game completes; the score is not reversed.
- **Computer strategy** follows: win → block → center → corner → fallback.
- **SQLite** is used for local demo persistence. No external database setup is needed.
