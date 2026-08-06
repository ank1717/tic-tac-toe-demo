using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services
{
    public interface IGameRulesEngine
    {
        (bool IsWin, int[]? WinningLine) CheckWin(char[] board, char player);
        bool CheckDraw(char[] board);
        int CalculateComputerMove(char[] board);
        char[] ReconstructBoard(IEnumerable<MoveLog> moves);
    }

    public class GameRulesEngine : IGameRulesEngine
    {
        // Explicitly defined 8 possible winning lines inside a type-safe jagged array matrix
        private static readonly int[][] WinLines = 
        [
            new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 }, // Horizontal Rows
            new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 }, // Vertical Columns
            new[] { 0, 4, 8 }, new[] { 2, 4, 6 }                    // Diagonals
        ];

        public char[] ReconstructBoard(IEnumerable<MoveLog> moves)
        {
            var board = new char[9];
            Array.Fill(board, '-');
            foreach (var move in moves.OrderBy(m => m.MoveNumber))
            {
                board[move.CellIndex] = move.Player;
            }
            return board;
        }

        public (bool IsWin, int[]? WinningLine) CheckWin(char[] board, char player)
        {
            foreach (var line in WinLines)
            {
                // CORRECTED FIXED LINES HERE: Use array index operators to extract positions
                int pos0 = line[0];
                int pos1 = line[1];
                int pos2 = line[2];

                // Ensure the cells match the specific active player symbol AND are NOT empty slots ('-')
                if (board[pos0] == player && 
                    board[pos1] == player && 
                    board[pos2] == player && 
                    player != '-')
                {
                    return (true, line);
                }
            }
            return (false, null);
        }

        public bool CheckDraw(char[] board) => !board.Contains('-');

        public int CalculateComputerMove(char[] board)
        {
            if (TryFindWinningOrBlockingMove(board, 'O', out int winIndex)) return winIndex;
            if (TryFindWinningOrBlockingMove(board, 'X', out int blockIndex)) return blockIndex;
            if (board[4] == '-') return 4;

            int[] corners = { 0, 2, 6, 8 };
            foreach (var corner in corners)
            {
                if (board[corner] == '-') return corner;
            }

            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == '-') return i;
            }

            return -1;
        }

        private static bool TryFindWinningOrBlockingMove(char[] board, char player, out int targetIndex)
        {
            targetIndex = -1;
            foreach (var line in WinLines)
            {
                int playerCount = 0;
                int emptyCount = 0;
                int localEmptyIndex = -1;

                foreach (var index in line)
                {
                    if (board[index] == player) playerCount++;
                    else if (board[index] == '-') { emptyCount++; localEmptyIndex = index; }
                }

                if (playerCount == 2 && emptyCount == 1)
                {
                    targetIndex = localEmptyIndex;
                    return true;
                }
            }
            return false;
        }
    }
}
