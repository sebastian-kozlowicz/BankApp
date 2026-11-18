import { TestBed } from '@angular/core/testing';
import { AppTestingModule } from '../testing/app-testing.module';
import { HomeComponent } from './home.component';
import { AuthService } from '../services/auth.service';

describe('HomeComponent', () => {
  it('renders Home without loading the customer profile for an anonymous visitor', () => {
    TestBed.configureTestingModule({ imports: [AppTestingModule] });
    sessionStorage.removeItem('token');
    const auth = TestBed.inject(AuthService);
    spyOn(auth, 'isCustomer').and.returnValue(false);
    const fixture = TestBed.createComponent(HomeComponent);
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('h1').textContent).toContain('Your money.');
    expect(fixture.nativeElement.querySelector('a[href="/registration/customer"]')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('a[href="/auth/login"]')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('app-customer-profile')).toBeNull();
  });

  it('links customers to their accounts without showing registration actions', () => {
    TestBed.configureTestingModule({ imports: [AppTestingModule] });
    const auth = TestBed.inject(AuthService);
    spyOn(auth, 'isLoggedIn').and.returnValue(true);
    spyOn(auth, 'isCustomer').and.returnValue(true);
    const fixture = TestBed.createComponent(HomeComponent);

    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('a[href="/customer/profile"]')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('a[href="/registration/customer"]')).toBeNull();
    expect(fixture.nativeElement.querySelector('app-customer-profile')).toBeNull();
  });

  it('keeps the home page visible to signed-in staff without customer actions', () => {
    TestBed.configureTestingModule({ imports: [AppTestingModule] });
    const auth = TestBed.inject(AuthService);
    spyOn(auth, 'isLoggedIn').and.returnValue(true);
    spyOn(auth, 'isCustomer').and.returnValue(false);
    const fixture = TestBed.createComponent(HomeComponent);

    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('h1')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('.session-note')).not.toBeNull();
    expect(fixture.nativeElement.querySelector('a[href="/registration/customer"]')).toBeNull();
    expect(fixture.nativeElement.querySelector('a[href="/customer/profile"]')).toBeNull();
  });
});
