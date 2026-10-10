import { Router } from '@angular/router';
import { AppTestingModule } from '../../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { BankTransferComponent } from './bank-transfer.component';

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
});
