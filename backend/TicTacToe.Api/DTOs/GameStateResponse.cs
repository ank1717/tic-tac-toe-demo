using TicTacToe.Api.Models;

namespace TicTacToe.Api.DTOs
{
    public class GameStateResponse
    {
        public int GameId { get; set; }
        public string BoardState { get; set; } = "---------";
        public string CurrentPlayer { get; set; } = "X";
        public string GameMode { get; set; } = "TwoPlayer";
        public string GameStatus { get; set; } = "InProgress";
        public string? Winner { get; set; }
        public List<int> WinningCells { get; set; } = [];
        public List<MoveDto> MoveHistory { get; set; } = [];
        public ScoreboardDto Scoreboard { get; set; } = new();
    }

    public class MoveDto
    {
        public int MoveNumber { get; set; }
        public string Player { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty; // Format: "Row X, Column Y"
    }

    public class ScoreboardDto
    {
        public int XWins { get; set; }
        public int OWins { get; set; }
        public int Draws { get; set; }
    }
}
