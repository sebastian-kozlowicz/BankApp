import { AppTestingModule } from '../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

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
});
