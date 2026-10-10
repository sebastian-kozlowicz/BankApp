import { AppTestingModule } from '../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { NavAdministratorMenuComponent } from './nav-administrator-menu.component';

describe('NavAdministratorMenuComponent', () => {
  let component: NavAdministratorMenuComponent;
  let fixture: ComponentFixture<NavAdministratorMenuComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
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
