using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Api.Models
{
    public class GameSession
    {
        [Key]
        public int Id { get; set; }
        public GameMode Mode { get; set; } = GameMode.TwoPlayer;
        public GameStatus Status { get; set; } = GameStatus.InProgress;
        public char CurrentTurn { get; set; } = 'X';
        public char? Winner { get; set; }
        public string WinningCells { get; set; } = string.Empty; // Comma separated indices e.g. "0,1,2"
        
        public ICollection<MoveLog> Moves { get; set; } = new List<MoveLog>();
    }
}
