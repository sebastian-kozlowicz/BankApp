import { Router } from '@angular/router';
import { AppTestingModule } from '../../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BankTransferComponent } from './bank-transfer.component';
import { BankTransferService } from '../../../services/bank-transfer.service';
import { EMPTY } from 'rxjs';

describe('BankTransferComponent', () => {
  let component: BankTransferComponent;
  let fixture: ComponentFixture<BankTransferComponent>;

  beforeEach(() => { history.replaceState(null, ''); });

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankTransferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('redirects direct navigation without transfer state to no-access', () => {
    const router = TestBed.inject(Router);
    const navigate = spyOn(router, 'navigate').and.resolveTo(true);
    component.ngOnInit();
    expect(navigate).toHaveBeenCalledWith(['/no-access'], { skipLocationChange: true });
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('re-enables the submit button when the transfer request completes', () => {
    const transferService = TestBed.inject(BankTransferService);
    spyOn(transferService, 'createBankTransfer').and.returnValue(EMPTY);
    component.requesterBankAccountId = 1;
    component.bankTransferForm.setValue({ receiverIban: 'PL00123456789012345678901234', value: 10 });

    component.submit();

    expect(component.isSubmitting()).toBeFalse();
  });
});
