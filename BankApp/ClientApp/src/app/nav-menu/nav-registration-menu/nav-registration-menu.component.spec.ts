import { AppTestingModule } from '../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { NavRegistrationMenuComponent } from './nav-registration-menu.component';

describe('NavRegistrationMenuComponent', () => {
  let component: NavRegistrationMenuComponent;
  let fixture: ComponentFixture<NavRegistrationMenuComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
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
