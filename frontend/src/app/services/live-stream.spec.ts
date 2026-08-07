import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { LiveStreamService } from './live-stream';

describe('LiveStreamService', () => {
  let service: LiveStreamService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection()]
    });
    service = TestBed.inject(LiveStreamService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('listenToStateStream returns an observable', () => {
    const stream = service.listenToStateStream();
    expect(stream).toBeDefined();
    expect(typeof stream.subscribe).toBe('function');
  });
});
