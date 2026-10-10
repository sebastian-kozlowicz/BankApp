import { AuthService } from '../../../services/auth.service';
import { AppTestingModule } from '../../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CustomerProfileComponent } from './customer-profile.component';

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
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
