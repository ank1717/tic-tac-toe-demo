import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { GameService } from './game';

describe('GameService', () => {
  let service: GameService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(GameService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('createNewSession posts to /api/games with mode param', () => {
    service.createNewSession('TwoPlayer').subscribe();
    const req = http.expectOne('http://localhost:5000/api/games?mode=TwoPlayer');
    expect(req.request.method).toBe('POST');
    req.flush({ gameId: 1, boardState: '---------', gameStatus: 'InProgress' });
  });

  it('submitCellMove posts to /api/games/:id/moves', () => {
    service.submitCellMove(1, 4).subscribe();
    const req = http.expectOne('http://localhost:5000/api/games/1/moves');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ cellIndex: 4 });
    req.flush({ gameId: 1, boardState: '----X----', gameStatus: 'InProgress' });
  });

  it('triggerUndo posts to /api/games/:id/undo', () => {
    service.triggerUndo(1).subscribe();
    const req = http.expectOne('http://localhost:5000/api/games/1/undo');
    expect(req.request.method).toBe('POST');
    req.flush({ gameId: 1, boardState: '---------', gameStatus: 'InProgress' });
  });

  it('resetCurrentBoard posts to /api/games/:id/reset', () => {
    service.resetCurrentBoard(1).subscribe();
    const req = http.expectOne('http://localhost:5000/api/games/1/reset');
    expect(req.request.method).toBe('POST');
    req.flush({ gameId: 1, boardState: '---------', gameStatus: 'InProgress' });
  });
});
