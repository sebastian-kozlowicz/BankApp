import { AppTestingModule } from '../testing/app-testing.module';
import { TestBed } from '@angular/core/testing';

import { BankAccountService } from './bank-account.service';
import { HttpTestingController } from '@angular/common/http/testing';
import { AccountType } from '../enumerators/accountType';
import { Currency } from '../enumerators/currency';

describe('BankAccountService', () => {
  let service: BankAccountService;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [AppTestingModule] });
    service = TestBed.inject(BankAccountService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('creates a bank account through the current-customer endpoint', () => {
    const requestModel = { accountType: AccountType.Checking, currency: Currency.Pln };
    service.createBankAccountForCurrentCustomer(requestModel).subscribe();

    const request = TestBed.inject(HttpTestingController)
      .expectOne('/api/BankAccounts/CreateForCurrentCustomer');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(requestModel);
    request.flush({});
  });
});
