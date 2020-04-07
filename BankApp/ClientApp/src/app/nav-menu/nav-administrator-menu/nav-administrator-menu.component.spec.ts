import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NavAdministratorMenuComponent } from './nav-administrator-menu.component';

describe('NavAdministratorMenuComponent', () => {
  let component: NavAdministratorMenuComponent;
  let fixture: ComponentFixture<NavAdministratorMenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NavAdministratorMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavAdministratorMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
