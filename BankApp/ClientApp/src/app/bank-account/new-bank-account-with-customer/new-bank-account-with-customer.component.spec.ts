import { AppTestingModule } from '../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { HttpTestingController } from '@angular/common/http/testing';

import { NewBankAccountWithCustomer } from "./new-bank-account-with-customer.component";

describe('NewBankAccountWithCustomer', () => {
  let component: NewBankAccountWithCustomer;
  let fixture: ComponentFixture<NewBankAccountWithCustomer>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewBankAccountWithCustomer);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('renders the registration heading, sign-in link and explicit step navigation', () => {
    expect(fixture.nativeElement.querySelector('h1')?.textContent).toContain('Create your BankApp account');
    expect(fixture.nativeElement.querySelector('a[href="/auth/login"]')).not.toBeNull();
    const next = fixture.nativeElement.querySelector('button[matStepperNext]') as HTMLButtonElement;
    expect(next).not.toBeNull();
    expect(next?.disabled).toBeTrue();
  });

  it('does not submit an incomplete registration', () => {
    expect(() => component.register()).not.toThrow();
    TestBed.inject(HttpTestingController).expectNone('/api/BankAccounts/CreateWithCustomerByCustomer');
    expect(component.password.touched).toBeTrue();
  });

  it('revalidates password confirmation when the original password changes', () => {
    component.passwordForm.setValue({ password: 'Test-only-123!', confirmPassword: 'Another-test-123!' });
    expect(component.confirmPassword.hasError('passwordMismatch')).toBeTrue();
    component.password.setValue('Another-test-123!');
    expect(component.passwordForm.valid).toBeTrue();
    component.password.setValue('Test-only-123!');
    expect(component.confirmPassword.hasError('passwordMismatch')).toBeTrue();
  });

  function fillValidRegistration() {
    component.accountInformationForm.setValue({ accountType: 1, currency: 0 });
    component.personalInformation.setValue({ name: 'Test', surname: 'Customer', email: 'customer@example.test', phoneNumber: '001234567' });
    component.address.setValue({ country: 'Poland', city: 'Poznan', street: 'Test street', houseNumber: '1', apartmentNumber: '2', postalCode: '01-001' });
    component.passwordForm.setValue({ password: 'Test-only-123!', confirmPassword: 'Test-only-123!' });
    fixture.detectChanges();
  }

  it('prevents duplicate submissions and clears the busy state after an HTTP error', () => {
    fillValidRegistration();
    const http = TestBed.inject(HttpTestingController);
    component.register();
    component.register();
    fixture.detectChanges();

    const requests = http.match('/api/BankAccounts/CreateWithCustomerByCustomer');
    expect(requests.length).toBe(1);
    expect(fixture.nativeElement.querySelector('.registration-panel').getAttribute('aria-busy')).toBe('true');
    requests.forEach(request => request.flush({}, { status: 500, statusText: 'Server error' }));
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.registration-panel').getAttribute('aria-busy')).toBe('false');
    expect(fixture.nativeElement.querySelector('[role="alert"]')?.textContent).toContain('Registration failed');
    http.verify();
  });

  it('shows a sign-in action after success and does not create the account again', () => {
    fillValidRegistration();
    const phone = fixture.nativeElement.querySelector('#phone-number') as HTMLInputElement;
    phone.value = '001234567';
    phone.dispatchEvent(new Event('input'));
    component.register();
    const http = TestBed.inject(HttpTestingController);
    const request = http.expectOne('/api/BankAccounts/CreateWithCustomerByCustomer');
    expect(request.request.body.bankAccount).toEqual({ accountType: 1, currency: 0 });
    expect(request.request.body.register.user.phoneNumber).toBe('001234567');
    expect(request.request.body.register.address.postalCode).toBe('01-001');
    request.flush({ id: 42 });
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.success-state')?.textContent).toContain('Your account is ready');
    expect(fixture.nativeElement.querySelector('.success-state a[href="/auth/login"]')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('mat-vertical-stepper')).toBeNull();
    component.register();
    http.expectNone('/api/BankAccounts/CreateWithCustomerByCustomer');
    http.verify();
  });
});
