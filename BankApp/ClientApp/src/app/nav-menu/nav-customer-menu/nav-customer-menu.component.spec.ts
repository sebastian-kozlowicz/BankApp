import { AppTestingModule } from '../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { NavCustomerMenuComponent } from './nav-customer-menu.component';

describe('NavCustomerMenuComponent', () => {
  let component: NavCustomerMenuComponent;
  let fixture: ComponentFixture<NavCustomerMenuComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavCustomerMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('links customers to their profile', () => {
    const profileLink = fixture.nativeElement.querySelector('a');

    expect(profileLink).not.toBeNull();
    expect(profileLink.getAttribute('href')).toBe('/customer/profile');
    expect(profileLink.textContent.trim()).toBe('Profile');
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
