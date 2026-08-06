using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Api.Models
{
    public class Scoreboard
    {
        [Key]
        public int Id { get; set; } // Single record tracking state
        public int XWins { get; set; } = 0;
        public int OWins { get; set; } = 0;
        public int Draws { get; set; } = 0;
    }
}
