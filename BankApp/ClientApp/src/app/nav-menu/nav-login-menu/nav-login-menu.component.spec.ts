import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { NavLoginMenuComponent } from './nav-login-menu.component';

describe('NavLoginMenuComponent', () => {
  let component: NavLoginMenuComponent;
  let fixture: ComponentFixture<NavLoginMenuComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ NavLoginMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavLoginMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
