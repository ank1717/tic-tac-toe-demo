import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { GameStateResponse } from '../../models/game.model';
import { GameService } from '../../services/game';
import { LiveStreamService } from '../../services/live-stream';

@Component({
  selector: 'app-game-board',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-board.html',
  styleUrls: ['./game-board.css']
})
export class GameBoardComponent implements OnInit {
  state: GameStateResponse | null = null;
  cells: string[] = Array(9).fill('-');
  activeMode = 'TwoPlayer';
  isBusy = false;
  isLoading = false;
  showChangeDialog = false;
  private lastClickAt = 0;

  constructor(
    private gameService: GameService,
    private liveStream: LiveStreamService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.liveStream.initializeSocketStream();

    // Read optional mode from query param and auto-start when present
    const modeFromQuery = this.route.snapshot.queryParamMap.get('mode');
    if (modeFromQuery) {
      this.initializeNewMatch(modeFromQuery);
      // start a short tick to let change detection update before creating session
      setTimeout(() => this.startGame(), 50);
    }

    this.liveStream.listenToStateStream().subscribe({
      next: (streamedState: GameStateResponse) => {
        this.applyState(streamedState);
        this.cdr.detectChanges();
      }
    });

    // Start in a mode selection state; require user to click Start to begin when no query param present
    if (!this.state) this.initializeNewMatch(this.activeMode);
  }

  initializeNewMatch(mode: string): void {
    // Only select the desired mode. Call startGame() to create a session.
    this.activeMode = mode;
    this.isBusy = false;
  }

  startGame(): void {
    this.isBusy = true;
    this.isLoading = true;
    this.gameService.createNewSession(this.activeMode).subscribe({
      next: (res) => {
        this.liveStream.bindToSessionGroup(res.gameId);
        this.applyState(res);
        this.isBusy = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error starting game session:', err);
        this.isBusy = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openChangeModeDialog(): void {
    this.showChangeDialog = true;
  }

  cancelChangeMode(): void {
    this.showChangeDialog = false;
  }

  changeModeTo(mode: string): void {
    this.showChangeDialog = false;
    this.activeMode = mode;
    this.isBusy = true;
    this.isLoading = true;
    // Create a new session with the selected mode (ends current match view)
    this.gameService.createNewSession(this.activeMode).subscribe({
      next: (res) => {
        this.liveStream.bindToSessionGroup(res.gameId);
        this.applyState(res);
        this.isBusy = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error changing mode:', err);
        this.isBusy = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onCellClick(index: number): void {
    if (!this.state || this.isBusy || this.cells[index] !== '-' || this.state.gameStatus !== 'InProgress') {
      return;
    }

    const now = Date.now();
    // ignore extremely rapid double-clicks
    if (this.lastClickAt && now - this.lastClickAt < 300) return;
    this.lastClickAt = now;

    this.isBusy = true;
    // update DOM immediately to block further clicks
    this.cdr.detectChanges();
    this.gameService.submitCellMove(this.state.gameId, index).subscribe({
      next: (res) => {
        this.applyState(res);
        this.isBusy = false;
        this.lastClickAt = 0;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error submitting move:', err);
        this.isBusy = false;
        this.lastClickAt = 0;
        this.cdr.detectChanges();
      }
    });
  }

  executeUndo(): void {
    if (!this.state || this.isBusy || this.state.moveHistory.length === 0 || this.state.gameStatus !== 'InProgress') {
      return;
    }

    this.isBusy = true;
    this.isLoading = true;
    this.gameService.triggerUndo(this.state.gameId).subscribe({
      next: (res) => {
        this.applyState(res);
        this.isBusy = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error undoing move:', err);
        this.isBusy = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  resetBoard(): void {
    if (!this.state || this.isBusy) return;

    this.isBusy = true;
    this.isLoading = true;
    this.gameService.resetCurrentBoard(this.state.gameId).subscribe({
      next: (res) => {
        this.applyState(res);
        this.isBusy = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error resetting game:', err);
        this.isBusy = false;
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  purgeScoreboard(): void {
    this.gameService.clearGlobalScoreboard().subscribe({
      next: (freshScore) => {
        if (this.state) {
          this.state.scoreboard = freshScore;
        }
      }
    });
  }

  isWinningCell(index: number): boolean {
    return this.state?.winningCells?.includes(index) ?? false;
  }

  private applyState(nextState: GameStateResponse): void {
    this.state = nextState;
    this.cells = Array.from(nextState.boardState);
  }
}
