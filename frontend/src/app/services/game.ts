import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GameStateResponse, ScoreboardDto } from '../models/game.model';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private readonly baseEndpoint = 'http://localhost:5000/api/games';
  private readonly scoreboardEndpoint = 'http://localhost:5000/api/scoreboard';

  constructor(private http: HttpClient) {}

  createNewSession(mode: string): Observable<GameStateResponse> {
    return this.http.post<GameStateResponse>(`${this.baseEndpoint}?mode=${encodeURIComponent(mode)}`, {});
  }

  submitCellMove(gameId: number, cellIndex: number): Observable<GameStateResponse> {
    return this.http.post<GameStateResponse>(`${this.baseEndpoint}/${gameId}/moves`, { cellIndex });
  }

  triggerUndo(gameId: number): Observable<GameStateResponse> {
    return this.http.post<GameStateResponse>(`${this.baseEndpoint}/${gameId}/undo`, {});
  }

  resetCurrentBoard(gameId: number): Observable<GameStateResponse> {
    return this.http.post<GameStateResponse>(`${this.baseEndpoint}/${gameId}/reset`, {});
  }

  fetchGlobalScoreboard(): Observable<ScoreboardDto> {
    return this.http.get<ScoreboardDto>(this.scoreboardEndpoint);
  }

  clearGlobalScoreboard(): Observable<ScoreboardDto> {
    return this.http.post<ScoreboardDto>(`${this.scoreboardEndpoint}/reset`, {});
  }
}
