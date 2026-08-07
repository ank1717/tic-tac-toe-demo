# Tic-Tac-Toe API Documentation

This document describes all backend HTTP endpoints and realtime hub contracts for the Tic-Tac-Toe Arena project.

## 1. API Basics

- Base URL (local): `http://localhost:5000`
- Content type: `application/json`
- Authentication: none
- CORS allowed origins:
  - `http://localhost:4200`
  - `http://127.0.0.1:4200`

## 2. Domain Values

### Game modes

- `TwoPlayer`
- `AgainstComputer`

### Game status values

- `InProgress`
- `Won`
- `Draw`

### Board representation

- `boardState` is a 9-character string.
- Indices map left-to-right, top-to-bottom.
- Empty cells are `-`, example: `X-O--O---`

Index map:

```text
0 | 1 | 2
3 | 4 | 5
6 | 7 | 8
```

## 3. Response Schemas

### GameStateResponse

```json
{
  "gameId": 1,
  "boardState": "X-O--O---",
  "currentPlayer": "X",
  "gameMode": "AgainstComputer",
  "gameStatus": "InProgress",
  "winner": null,
  "winningCells": [],
  "moveHistory": [
    {
      "moveNumber": 1,
      "player": "X",
      "position": "Row 1, Column 1"
    }
  ],
  "scoreboard": {
    "xWins": 2,
    "oWins": 1,
    "draws": 3
  }
}
```

Notes:
- `winner` is `"X"` or `"O"` for a win.
- For a draw, `winner` is `"D"`.
- `winningCells` contains board indices for the winning line, example: `[0,1,2]`.

### ScoreboardDto

```json
{
  "xWins": 2,
  "oWins": 1,
  "draws": 3
}
```

## 4. Endpoints

### 4.1 Create game session

- Method: `POST`
- Path: `/api/games`
- Query params:
  - `mode` (optional): `TwoPlayer` or `AgainstComputer`
  - Default mode: `TwoPlayer`

Example:

```bash
curl -X POST "http://localhost:5000/api/games?mode=AgainstComputer"
```

Success response:
- Status: `200 OK`
- Body: `GameStateResponse`

Errors:
- `400 Bad Request` if invalid enum value is passed for `mode`

### 4.2 Get game state

- Method: `GET`
- Path: `/api/games/{id}`

Example:

```bash
curl "http://localhost:5000/api/games/1"
```

Success response:
- Status: `200 OK`
- Body: `GameStateResponse`

Errors:
- `404 Not Found` with message `Session not found.`

### 4.3 Submit move

- Method: `POST`
- Path: `/api/games/{id}/moves`
- Body accepts either:
  - `cellIndex` (0..8), or
  - `row` and `column` (1..3)

Request examples:

```json
{ "cellIndex": 4 }
```

```json
{ "row": 2, "column": 2 }
```

Behavior details:
- If both styles are provided, valid `cellIndex` is used first.
- Move is rejected when the game is complete.
- Move is rejected for occupied or out-of-range cells.
- In `AgainstComputer` mode, backend may auto-play `O` in the same request cycle.
- After processing, updated state is returned and broadcast to SignalR group.

Success response:
- Status: `200 OK`
- Body: `GameStateResponse`

Errors:
- `404 Not Found` with message `Active match not found.`
- `400 Bad Request` with one of:
  - `Game is already completed.`
  - `Provide a valid cell index.`
  - `Invalid or occupied cell selection.`

### 4.4 Undo last move

- Method: `POST`
- Path: `/api/games/{id}/undo`

Behavior details:
- `TwoPlayer`: removes last 1 move.
- `AgainstComputer`: removes last 2 moves when possible.
- Undo is disabled once game status is not `InProgress`.

Success response:
- Status: `200 OK`
- Body: `GameStateResponse`

Errors:
- `404 Not Found` if game session does not exist
- `400 Bad Request` with one of:
  - `Undo is disabled once the match completes.`
  - `No historical actions logged yet.`

### 4.5 Reset current board

- Method: `POST`
- Path: `/api/games/{id}/reset`

Behavior details:
- Clears all moves in the session.
- Resets game status to `InProgress`.
- Resets turn to `X`.
- Keeps global scoreboard unchanged.

Success response:
- Status: `200 OK`
- Body: `GameStateResponse`

Errors:
- `404 Not Found` if game session does not exist

### 4.6 Get scoreboard

- Method: `GET`
- Path: `/api/scoreboard`

Success response:
- Status: `200 OK`
- Body: `ScoreboardDto`

Behavior details:
- If scoreboard row does not exist yet, backend creates it.

### 4.7 Reset scoreboard

- Method: `POST`
- Path: `/api/scoreboard/reset`

Success response:
- Status: `200 OK`
- Body: `ScoreboardDto` (all counters set to `0`)

## 5. Realtime Hub (SignalR)

- Hub path: `/hub/game`

### Client -> server methods

- `JoinSession(int gameId)`
  - Adds connection to group `Game_{gameId}`
- `LeaveSession(int gameId)`
  - Removes connection from group `Game_{gameId}`

### Server -> client events

- `ReceiveGameState(gameState)`
  - Emitted after move, undo, and reset operations
  - Payload shape matches `GameStateResponse`
- `SystemNotification(announcement)`
  - Informational text event when joining group

### Typical realtime flow

1. Create or open game via REST.
2. Connect to SignalR hub.
3. Call `JoinSession(gameId)`.
4. Send REST commands for moves/undo/reset.
5. Listen to `ReceiveGameState` to refresh UI instantly.

## 6. Endpoint Summary Table

| Method | Path | Purpose |
|---|---|---|
| POST | `/api/games?mode=TwoPlayer|AgainstComputer` | Create a new game session |
| GET | `/api/games/{id}` | Fetch full game state |
| POST | `/api/games/{id}/moves` | Submit a player move |
| POST | `/api/games/{id}/undo` | Undo latest move(s) by mode |
| POST | `/api/games/{id}/reset` | Reset board for session |
| GET | `/api/scoreboard` | Get global scoreboard |
| POST | `/api/scoreboard/reset` | Reset global scoreboard |
| SignalR | `/hub/game` | Realtime state updates |

## 7. Recommended API Test Sequence

1. `POST /api/games?mode=AgainstComputer`
2. `POST /api/games/{id}/moves` with `{ "cellIndex": 0 }`
3. `GET /api/games/{id}` to verify state and history
4. `POST /api/games/{id}/undo`
5. `POST /api/games/{id}/reset`
6. `GET /api/scoreboard`
7. `POST /api/scoreboard/reset`

This sequence validates the complete gameplay lifecycle and scoreboard behavior.
