import { AppTestingModule } from '../testing/app-testing.module';
import { TestBed } from '@angular/core/testing';

import { AuthGuard } from './auth-guard.service';

describe('AuthGuard', () => {
  let service: AuthGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [AppTestingModule] });
    service = TestBed.inject(AuthGuard);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
