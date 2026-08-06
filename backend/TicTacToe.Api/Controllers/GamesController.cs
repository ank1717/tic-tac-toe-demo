using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using TicTacToe.Api.Data;
using TicTacToe.Api.DTOs;
using TicTacToe.Api.Models;
using TicTacToe.Api.Services;
using TicTacToe.Api.Hubs;

namespace TicTacToe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly IGameRulesEngine _rules;
        private readonly IHubContext<GameHub> _hubContext;

        public GamesController(GameDbContext context, IGameRulesEngine rules, IHubContext<GameHub> hubContext)
        {
            _context = context;
            _rules = rules;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<ActionResult<GameStateResponse>> CreateGame([FromQuery] GameMode mode = GameMode.TwoPlayer)
        {
            var session = new GameSession
            {
                Mode = mode,
                Status = GameStatus.InProgress,
                CurrentTurn = 'X'
            };

            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(await BuildStateResponse(session.Id));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameStateResponse>> GetGame(int id)
        {
            var session = await _context.GameSessions.FindAsync(id);
            if (session == null) return NotFound("Session not found.");
            return Ok(await BuildStateResponse(id));
        }

        [HttpPost("{id}/moves")]
        public async Task<ActionResult<GameStateResponse>> MakeMove(int id, [FromBody] MoveRequest request)
        {
            var session = await _context.GameSessions.Include(g => g.Moves).FirstOrDefaultAsync(g => g.Id == id);
            if (session == null) return NotFound("Active match not found.");
            if (session.Status != GameStatus.InProgress) return BadRequest("Game is already completed.");

            var cellIndex = ResolveCellIndex(request);
            if (cellIndex is null) return BadRequest("Provide a valid cell index.");

            var board = _rules.ReconstructBoard(session.Moves);
            if (cellIndex < 0 || cellIndex > 8 || board[cellIndex.Value] != '-')
            {
                return BadRequest("Invalid or occupied cell selection.");
            }

            var activePlayer = session.CurrentTurn;
            ExecuteMoveOnSession(session, cellIndex.Value, activePlayer);
            board[cellIndex.Value] = activePlayer;

            if (EvaluateMatchEnding(session, board, activePlayer))
            {
                await _context.SaveChangesAsync();
                return await BroadcastStateAsync(id);
            }


            if (session.Mode == GameMode.AgainstComputer && session.Status == GameStatus.InProgress)
            {
                var computerMoveIndex = _rules.CalculateComputerMove(board);
                if (computerMoveIndex != -1)
                {
                    ExecuteMoveOnSession(session, computerMoveIndex, 'O');
                    board[computerMoveIndex] = 'O';
                    EvaluateMatchEnding(session, board, 'O');
                }
            }
            if (session.Mode == GameMode.AgainstComputer && session.Status == GameStatus.InProgress)
            {
                // Add a slight delay so the UI can show the human move before the computer responds
                await Task.Delay(600);
                var computerMoveIndex = _rules.CalculateComputerMove(board);
                if (computerMoveIndex != -1)
                {
                    ExecuteMoveOnSession(session, computerMoveIndex, 'O');
                    board[computerMoveIndex] = 'O';
                    EvaluateMatchEnding(session, board, 'O');
                }
            }

            await _context.SaveChangesAsync();
            return await BroadcastStateAsync(id);
        }

        [HttpPost("{id}/undo")]
        public async Task<ActionResult<GameStateResponse>> UndoLastMove(int id)
        {
            var session = await _context.GameSessions.Include(g => g.Moves).FirstOrDefaultAsync(g => g.Id == id);
            if (session == null) return NotFound();
            if (session.Status != GameStatus.InProgress) return BadRequest("Undo is disabled once the match completes.");
            if (!session.Moves.Any()) return BadRequest("No historical actions logged yet.");

            if (session.Mode == GameMode.TwoPlayer)
            {
                var lastMove = session.Moves.OrderByDescending(m => m.MoveNumber).First();
                _context.MoveLogs.Remove(lastMove);
                session.CurrentTurn = lastMove.Player;
            }
            else
            {
                var targetingMoves = session.Moves.OrderByDescending(m => m.MoveNumber).Take(2).ToList();
                foreach (var move in targetingMoves) _context.MoveLogs.Remove(move);
                session.CurrentTurn = 'X';
            }

            await _context.SaveChangesAsync();
            return await BroadcastStateAsync(id);
        }

        [HttpPost("{id}/reset")]
        public async Task<ActionResult<GameStateResponse>> ResetGame(int id)
        {
            var session = await _context.GameSessions.Include(g => g.Moves).FirstOrDefaultAsync(g => g.Id == id);
            if (session == null) return NotFound();

            _context.MoveLogs.RemoveRange(session.Moves);
            session.Status = GameStatus.InProgress;
            session.CurrentTurn = 'X';
            session.Winner = null;
            session.WinningCells = string.Empty;

            await _context.SaveChangesAsync();
            return await BroadcastStateAsync(id);
        }

        [HttpGet("/api/scoreboard")]
        public async Task<ActionResult<ScoreboardDto>> GetScoreboard()
        {
            var scores = await GetOrCreateScoreboardAsync();
            return Ok(new ScoreboardDto { XWins = scores.XWins, OWins = scores.OWins, Draws = scores.Draws });
        }

        [HttpPost("/api/scoreboard/reset")]
        public async Task<ActionResult<ScoreboardDto>> ResetScoreboard()
        {
            var scores = await GetOrCreateScoreboardAsync();
            scores.XWins = 0; scores.OWins = 0; scores.Draws = 0;
            await _context.SaveChangesAsync();
            return Ok(new ScoreboardDto
            {
                XWins = scores.XWins,
                OWins = scores.OWins,
                Draws = scores.Draws
            });
        }

        private int? ResolveCellIndex(MoveRequest? request)
        {
            if (request is null) return null;
            if (request.CellIndex is not null && request.CellIndex.Value is >= 0 and <= 8) return request.CellIndex.Value;
            if (request.Row is not null && request.Column is not null)
            {
                var row = request.Row.Value;
                var column = request.Column.Value;
                if (row is < 1 or > 3 || column is < 1 or > 3) return null;
                return (row - 1) * 3 + (column - 1);
            }

            return null;
        }

        private static void ExecuteMoveOnSession(GameSession session, int index, char player)
        {
            session.Moves.Add(new MoveLog
            {
                MoveNumber = session.Moves.Count + 1,
                Player = player,
                CellIndex = index
            });
            session.CurrentTurn = player == 'X' ? 'O' : 'X';
        }

        private bool EvaluateMatchEnding(GameSession session, char[] board, char activePlayer)
        {
            var (isWin, winningLine) = _rules.CheckWin(board, activePlayer);
            if (isWin && winningLine != null)
            {
                session.Status = GameStatus.Won;
                session.Winner = activePlayer;
                session.WinningCells = string.Join(",", winningLine);
                UpdateScoreboardCache(activePlayer);
                return true;
            }
            if (_rules.CheckDraw(board))
            {
                session.Status = GameStatus.Draw;
                session.Winner = 'D';
                UpdateScoreboardCache('D');
                return true;
            }
            return false;
        }

        private async Task<ActionResult<GameStateResponse>> BroadcastStateAsync(int gameId)
        {
            var state = await BuildStateResponse(gameId);
            await _hubContext.Clients.Group($"Game_{gameId}").SendAsync("ReceiveGameState", state);
            return Ok(state);
        }

        private void UpdateScoreboardCache(char endingState)
        {
            var score = _context.Scoreboards.FirstOrDefault() ?? new Scoreboard();
            if (_context.Entry(score).State == EntityState.Detached) _context.Scoreboards.Add(score);

            if (endingState == 'X') score.XWins++;
            else if (endingState == 'O') score.OWins++;
            else score.Draws++;
        }

        private async Task<Scoreboard> GetOrCreateScoreboardAsync()
        {
            var item = await _context.Scoreboards.FirstOrDefaultAsync();
            if (item == null)
            {
                item = new Scoreboard();
                _context.Scoreboards.Add(item);
                await _context.SaveChangesAsync();
            }
            return item;
        }

        private async Task<GameStateResponse> BuildStateResponse(int gameId)
        {
            var session = await _context.GameSessions.Include(g => g.Moves).FirstAsync(g => g.Id == gameId);
            var scores = await GetOrCreateScoreboardAsync();
            var reconstructedBoard = _rules.ReconstructBoard(session.Moves);

            return new GameStateResponse
            {
                GameId = session.Id,
                BoardState = new string(reconstructedBoard),
                CurrentPlayer = session.CurrentTurn.ToString(),
                GameMode = session.Mode.ToString(),
                GameStatus = session.Status.ToString(),
                Winner = session.Winner?.ToString(),
                WinningCells = string.IsNullOrEmpty(session.WinningCells)
                    ? new List<int>()
                    : session.WinningCells.Split(',').Select(int.Parse).ToList(),
                Scoreboard = new ScoreboardDto { XWins = scores.XWins, OWins = scores.OWins, Draws = scores.Draws },
                MoveHistory = session.Moves.OrderBy(m => m.MoveNumber).Select(m => new MoveDto
                {
                    MoveNumber = m.MoveNumber,
                    Player = m.Player.ToString(),
                    Position = $"Row {(m.CellIndex / 3) + 1}, Column {(m.CellIndex % 3) + 1}"
                }).ToList()
            };
        }
    }
}
