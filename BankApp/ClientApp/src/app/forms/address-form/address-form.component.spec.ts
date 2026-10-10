import { AppTestingModule } from '../../testing/app-testing.module';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AddressFormComponent as AddressFormComponent } from './address-form.component';

describe('AddressFormComponent', () => {
  let component: AddressFormComponent;
  let fixture: ComponentFixture<AddressFormComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      imports: [AppTestingModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddressFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('preserves postal codes as JSON strings including hyphens and leading zeros', () => {
    const input: HTMLInputElement = fixture.nativeElement.querySelector('#postal-code');

    for (const value of ['62-923', '00-001', '00123', 'SW1A 1AA']) {
      input.value = value;
      input.dispatchEvent(new Event('input'));
      fixture.detectChanges();

      expect(component.postalCode.value).withContext(value).toBe(value);
      expect(component.postalCode.valid).withContext(value).toBeTrue();
      expect(JSON.parse(JSON.stringify(component.addressFormValue)).postalCode)
        .withContext(value).toBe(value);
    }

    input.value = '';
    input.dispatchEvent(new Event('input'));
    expect(component.postalCode.hasError('required')).toBeTrue();
  });
});
