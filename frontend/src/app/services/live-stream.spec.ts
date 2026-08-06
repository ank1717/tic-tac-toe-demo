import { TestBed } from '@angular/core/testing';

import { LiveStream } from './live-stream';

describe('LiveStream', () => {
  let service: LiveStream;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LiveStream);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
