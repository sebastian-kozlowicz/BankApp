import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { AppTestingModule } from '../../../testing/app-testing.module';
import { AccountType } from '../../../enumerators/accountType';
import { Currency } from '../../../enumerators/currency';
import { BankAccountService } from '../../../services/bank-account.service';
import { CustomerNewBankAccountComponent } from './customer-new-bank-account.component';

describe('CustomerNewBankAccountComponent', () => {
  let component: CustomerNewBankAccountComponent;
  let fixture: ComponentFixture<CustomerNewBankAccountComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({ imports: [AppTestingModule] }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerNewBankAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('creates an account for the current customer and returns to profile', () => {
    const bankAccountService = TestBed.inject(BankAccountService);
    const createAccount = spyOn(bankAccountService, 'createBankAccountForCurrentCustomer')
      .and.returnValue(of({} as any));
    const navigate = spyOn(TestBed.inject(Router), 'navigate').and.resolveTo(true);
    component.accountForm.setValue({ accountType: AccountType.Checking, currency: Currency.Pln });

    component.createAccount();

    expect(createAccount).toHaveBeenCalledWith({ accountType: AccountType.Checking, currency: Currency.Pln });
    expect(navigate).toHaveBeenCalledWith(['/customer/profile']);
  });
});
