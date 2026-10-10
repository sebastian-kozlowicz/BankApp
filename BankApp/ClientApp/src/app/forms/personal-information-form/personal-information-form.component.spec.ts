import { AppTestingModule } from '../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PersonalInformationFormComponent } from './personal-information-form.component';

describe('PersonalInformationFormComponent', () => {
  let component: PersonalInformationFormComponent;
  let fixture: ComponentFixture<PersonalInformationFormComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonalInformationFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('preserves a phone number with leading zeros as a JSON string', () => {
    const input: HTMLInputElement = fixture.nativeElement.querySelector('#phone-number');
    input.value = '001234567';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    expect(component.phoneNumber.value).toBe('001234567');
    expect(component.phoneNumber.valid).toBeTrue();
    expect(JSON.parse(JSON.stringify(component.personalInformationFormValue)).phoneNumber)
      .toBe('001234567');
  });

  it('validates phone number length according to the API contract', () => {
    for (const value of ['000000', '123456789012', '+48123456789']) {
      component.phoneNumber.setValue(value);
      expect(component.phoneNumber.valid).withContext(value).toBeTrue();
    }

    for (const value of ['', '12345', '1234567890123']) {
      component.phoneNumber.setValue(value);
      expect(component.phoneNumber.valid).withContext(value).toBeFalse();
    }
  });
});
