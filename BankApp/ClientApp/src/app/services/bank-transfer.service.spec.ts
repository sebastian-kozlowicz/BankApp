import { AppTestingModule } from '../testing/app-testing.module';
import { TestBed } from '@angular/core/testing';

import { BankTransferService } from './bank-transfer.service';

describe('BankTransferService', () => {
  let service: BankTransferService;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [AppTestingModule] });
    service = TestBed.inject(BankTransferService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
