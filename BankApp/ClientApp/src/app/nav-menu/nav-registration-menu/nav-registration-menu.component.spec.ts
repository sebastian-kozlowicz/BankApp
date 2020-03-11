import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NavRegistrationMenuComponent } from './nav-registration-menu.component';

describe('NavRegistrationMenuComponent', () => {
  let component: NavRegistrationMenuComponent;
  let fixture: ComponentFixture<NavRegistrationMenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NavRegistrationMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavRegistrationMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
