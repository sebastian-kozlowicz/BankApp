import { TestBed } from '@angular/core/testing';
import { HttpTestingController } from '@angular/common/http/testing';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AppTestingModule } from '../testing/app-testing.module';
import { AuthService } from '../services/auth.service';
import { NavMenuComponent } from './nav-menu.component';

describe('NavMenuComponent authentication actions', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [AppTestingModule] });
  });

  it('shows login and registration to guests', () => {
    const auth = TestBed.inject(AuthService);
    spyOn(auth, 'isLoggedIn').and.returnValue(false);
    const fixture = TestBed.createComponent(NavMenuComponent);

    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('a[href="/auth/login"]')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('a[href="/registration/customer"]')).not.toBeNull();
    expect(fixture.nativeElement.textContent).not.toContain('Logout');
  });

  it('updates guest actions after login and restores them after logout', () => {
    const auth = TestBed.inject(AuthService);
    const http = TestBed.inject(HttpTestingController);
    const jwt = TestBed.inject(JwtHelperService);
    let token: string | null = null;
    spyOn(jwt, 'tokenGetter').and.callFake(() => token);
    spyOn(jwt, 'decodeToken').and.returnValue({ role: ['Customer'] });
    spyOn(sessionStorage, 'setItem').and.callFake((_key, value) => { token = value; });
    spyOn(sessionStorage, 'removeItem').and.callFake(() => { token = null; });
    const fixture = TestBed.createComponent(NavMenuComponent);
    fixture.detectChanges();

    auth.login({ email: 'customer@example.test', password: 'test-only-password' }).subscribe();
    http.expectOne('/api/auth/login').flush({ token: 'test-only-token' });
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('a[href="/auth/login"]')).toBeNull();
    expect(fixture.nativeElement.querySelector('a[href="/registration/customer"]')).toBeNull();
    expect(fixture.nativeElement.textContent).toContain('Logout');

    auth.logout();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('a[href="/auth/login"]')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('a[href="/registration/customer"]')).not.toBeNull();
    expect(fixture.nativeElement.textContent).not.toContain('Logout');
    http.verify();
  });
});
