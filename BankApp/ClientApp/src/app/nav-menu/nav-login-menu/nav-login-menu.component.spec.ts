import { AppTestingModule } from '../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { NavLoginMenuComponent } from './nav-login-menu.component';

describe('NavLoginMenuComponent', () => {
  let component: NavLoginMenuComponent;
  let fixture: ComponentFixture<NavLoginMenuComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
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
