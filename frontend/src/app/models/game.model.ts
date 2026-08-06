export interface MoveDto {
  moveNumber: number;
  player: string;
  position: string;
}

export interface ScoreboardDto {
  xWins: number;
  oWins: number;
  draws: number;
}

export interface GameStateResponse {
  gameId: number;
  boardState: string;
  currentPlayer: string;
  gameMode: string;
  gameStatus: string;
  winner: string | null;
  winningCells: number[];
  moveHistory: MoveDto[];
  scoreboard: ScoreboardDto;
}
