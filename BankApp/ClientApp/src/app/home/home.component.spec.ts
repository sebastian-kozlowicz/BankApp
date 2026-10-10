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
    expect(fixture.nativeElement.textContent).toContain('Home');
    expect(fixture.nativeElement.querySelector('app-customer-profile')).toBeNull();
  });
});
