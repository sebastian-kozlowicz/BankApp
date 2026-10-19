import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AppTestingModule } from '../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('returns non-customer users to Home after login', () => {
    const auth = TestBed.inject(AuthService);
    spyOn(auth, 'login').and.returnValue(of(true));
    spyOn(auth, 'isCustomer').and.returnValue(false);
    const navigate = spyOn(TestBed.inject(Router), 'navigate').and.resolveTo(true);
    component.loginFormModel.setValue({ email: 'user@example.com', password: 'password' });
    component.onSubmit();
    expect(navigate).toHaveBeenCalledWith(['/']);
  });

  it('re-enables the form after a failed login request', () => {
    const auth = TestBed.inject(AuthService);
    spyOn(auth, 'login').and.returnValue(throwError(() => ({ status: 400 })));
    component.loginFormModel.setValue({ email: 'user@example.com', password: 'invalid' });

    component.onSubmit();

    expect(component.isSubmitting()).toBeFalse();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
