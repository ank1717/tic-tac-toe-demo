import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { GameBoardComponent } from './game-board';

const activatedRouteStub = {
  snapshot: { queryParamMap: { get: () => null } }
};

describe('GameBoardComponent', () => {
  let component: GameBoardComponent;
  let fixture: ComponentFixture<GameBoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GameBoardComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ActivatedRoute, useValue: activatedRouteStub }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(GameBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('initial state has 9 empty cells', () => {
    expect(component.cells.length).toBe(9);
    expect(component.cells.every(c => c === '-')).toBeTrue();
  });

  it('initial state has no active game session', () => {
    expect(component.state).toBeNull();
  });

  it('isBusy starts false', () => {
    expect(component.isBusy).toBeFalse();
  });

  it('isWinningCell returns false when no game state', () => {
    expect(component.isWinningCell(0)).toBeFalse();
  });

  it('isWinningCell returns false for non-winning cell', () => {
    component['state'] = {
      gameId: 1, boardState: 'X--------', currentPlayer: 'O',
      gameMode: 'TwoPlayer', gameStatus: 'InProgress',
      winner: null, winningCells: [], moveHistory: [],
      scoreboard: { xWins: 0, oWins: 0, draws: 0 }
    } as any;
    expect(component.isWinningCell(1)).toBeFalse();
  });

  it('onCellClick does nothing when no state', () => {
    expect(() => component.onCellClick(0)).not.toThrow();
  });

  it('onCellClick does nothing when game is not InProgress', () => {
    component['state'] = {
      gameId: 1, boardState: 'XXX------', currentPlayer: 'O',
      gameMode: 'TwoPlayer', gameStatus: 'Won',
      winner: 'X', winningCells: [0, 1, 2], moveHistory: [],
      scoreboard: { xWins: 1, oWins: 0, draws: 0 }
    } as any;
    component.cells = Array.from('XXX------');
    const busyBefore = component.isBusy;
    component.onCellClick(4);
    expect(component.isBusy).toBe(busyBefore); // should not have changed
  });

  it('goHome navigates without throwing', () => {
    expect(() => component.goHome()).not.toThrow();
  });
});
