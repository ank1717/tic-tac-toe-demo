namespace TicTacToe.Api.DTOs
{
    public class MoveRequest
    {
        public int? CellIndex { get; set; }
        public int? Row { get; set; }
        public int? Column { get; set; }
    }
}
