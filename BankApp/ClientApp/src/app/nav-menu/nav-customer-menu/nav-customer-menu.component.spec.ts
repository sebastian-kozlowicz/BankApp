import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { NavCustomerMenuComponent } from './nav-customer-menu.component';

describe('NavCustomerMenuComponent', () => {
  let component: NavCustomerMenuComponent;
  let fixture: ComponentFixture<NavCustomerMenuComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ NavCustomerMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavCustomerMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
