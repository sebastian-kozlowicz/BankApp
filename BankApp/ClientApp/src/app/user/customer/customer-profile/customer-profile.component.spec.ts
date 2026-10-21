import { AuthService } from '../../../services/auth.service';
import { AppTestingModule } from '../../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CustomerProfileComponent } from './customer-profile.component';
import { BankAccountService } from '../../../services/bank-account.service';
import { throwError } from 'rxjs';

describe('CustomerProfileComponent', () => {
  let component: CustomerProfileComponent;
  let fixture: ComponentFixture<CustomerProfileComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerProfileComponent);
    component = fixture.componentInstance;
    spyOnProperty(TestBed.inject(AuthService), 'currentUser', 'get').and.returnValue({ userId: 1 } as any);
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('renders an account opening action when the API returns 404', () => {
    spyOn(TestBed.inject(BankAccountService), 'getBankAccounts')
      .and.returnValue(throwError(() => ({ status: 404 })));

    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(component.errorMessage()).toBe('');
    expect(element.textContent).toContain("You don't have a bank account yet");
    expect(element.querySelector<HTMLAnchorElement>('.open-account-button')?.getAttribute('href'))
      .toBe('/customer/accounts/new');
  });
});
