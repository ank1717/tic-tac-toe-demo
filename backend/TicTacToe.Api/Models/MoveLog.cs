using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Api.Models
{
    public class MoveLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int GameSessionId { get; set; }
        
        [ForeignKey("GameSessionId")]
        public GameSession? GameSession { get; set; }

        public int MoveNumber { get; set; }
        public char Player { get; set; } // 'X' or 'O'
        public int CellIndex { get; set; } // 0-8 mapping to 3x3 layout
    }
}
