import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewBankAccountWithCustomer } from "./new-bank-account-with-customer.component";

describe('NewBankAccountWithCustomer', () => {
  let component: NewBankAccountWithCustomer;
  let fixture: ComponentFixture<NewBankAccountWithCustomer>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewBankAccountWithCustomer ]
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
