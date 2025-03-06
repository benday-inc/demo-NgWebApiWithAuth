import { TestBed } from '@angular/core/testing';

import { SecuredPageService } from './secured-page.service';

describe('SecuredPageService', () => {
  let service: SecuredPageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SecuredPageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
