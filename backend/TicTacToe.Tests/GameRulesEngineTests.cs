using TicTacToe.Api.Models;
using TicTacToe.Api.Services;

namespace TicTacToe.Tests;

public class GameRulesEngineTests
{
    private readonly GameRulesEngine _engine = new();

    [Fact]
    public void ReconstructBoard_BuildsBoardFromMovesInOrder()
    {
        var moves = new List<MoveLog>
        {
            new() { MoveNumber = 1, Player = 'X', CellIndex = 0 },
            new() { MoveNumber = 2, Player = 'O', CellIndex = 4 }
        };

        var board = _engine.ReconstructBoard(moves);

        Assert.Equal('X', board[0]);
        Assert.Equal('O', board[4]);
        Assert.Equal('-', board[1]);
    }

    [Fact]
    public void CheckWin_ReturnsTrueForRowWin()
    {
        var board = new[] { 'X', 'X', 'X', '-', '-', '-', '-', '-', '-' };

        var result = _engine.CheckWin(board, 'X');

        Assert.True(result.IsWin);
        Assert.Equal(new[] { 0, 1, 2 }, result.WinningLine);
    }

    [Fact]
    public void CheckWin_ReturnsTrueForColumnWin()
    {
        var board = new[] { 'O', '-', '-', 'O', '-', '-', 'O', '-', '-' };

        var result = _engine.CheckWin(board, 'O');

        Assert.True(result.IsWin);
        Assert.Equal(new[] { 0, 3, 6 }, result.WinningLine);
    }

    [Fact]
    public void CheckWin_ReturnsTrueForDiagonalWin()
    {
        var board = new[] { 'X', '-', '-', '-', 'X', '-', '-', '-', 'X' };

        var result = _engine.CheckWin(board, 'X');

        Assert.True(result.IsWin);
        Assert.Equal(new[] { 0, 4, 8 }, result.WinningLine);
    }

    [Fact]
    public void CheckDraw_ReturnsTrueWhenBoardIsFullWithoutWinner()
    {
        var board = new[] { 'X', 'O', 'X', 'X', 'O', 'O', 'O', 'X', 'X' };

        Assert.True(_engine.CheckDraw(board));
    }

    [Fact]
    public void CalculateComputerMove_PrefersWinningMove()
    {
        var board = new[] { 'O', 'O', '-', '-', '-', '-', '-', '-', '-' };

        var move = _engine.CalculateComputerMove(board);

        Assert.Equal(2, move);
    }

    [Fact]
    public void CalculateComputerMove_BlocksOpponentWinningMove()
    {
        var board = new[] { 'X', 'X', '-', '-', '-', '-', '-', '-', '-' };

        var move = _engine.CalculateComputerMove(board);

        Assert.Equal(2, move);
    }
}
