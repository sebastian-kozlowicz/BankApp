import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankAccountSummaryComponent } from './bank-account-summary.component';

describe('BankAccountSummaryComponent', () => {
  let component: BankAccountSummaryComponent;
  let fixture: ComponentFixture<BankAccountSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankAccountSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankAccountSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
