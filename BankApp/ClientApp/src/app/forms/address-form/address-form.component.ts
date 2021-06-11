import { Component, forwardRef } from '@angular/core';
import { FormBuilder, Validators, ControlValueAccessor, NG_VALUE_ACCESSOR, NG_VALIDATORS, FormControl } from
  '@angular/forms';
import { Subscription } from 'rxjs';
import { AddressFormValues } from '../../interfaces/forms/address-form-values';
import { COUNTRIES } from '../../constants/app-constants';

@Component({
  selector: 'app-address-form',
  templateUrl: './address-form.component.html',
  styleUrls: ['./address-form.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AddressFormComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => AddressFormComponent),
      multi: true
    }
  ]
})
export class AddressFormComponent implements ControlValueAccessor {

  constructor(private fb: FormBuilder) {
    this.subscriptions.push(
      this.addressForm.valueChanges.subscribe(value => {
        this.onChange(value);
        this.onTouched();
      })
    );
  }

  subscriptions: Subscription[] = [];
  countries = COUNTRIES;

  get addressFormValue(): AddressFormValues {
    return this.addressForm.value;
  }

  set addressFormValue(value: AddressFormValues) {
    this.addressForm.setValue(value);
    this.onChange(value);
    this.onTouched();
  }

  get street() {
    return this.addressForm.get('street');
  }

  get houseNumber() {
    return this.addressForm.get('houseNumber');
  }

  get apartmentNumber() {
    return this.addressForm.get('apartmentNumber');
  }

  get postalCode() {
    return this.addressForm.get('postalCode');
  }

  get city() {
    return this.addressForm.get('city');
  }

  get country() {
    return this.addressForm.get('country');
  }

  addressForm = this.fb.group({
    street: ['', Validators.required],
    houseNumber: ['', Validators.required],
    apartmentNumber: ['', Validators.required],
    postalCode: ['', Validators.required],
    city: ['', Validators.required],
    country: ['', Validators.required]
  });

  validate(_: FormControl) {
    return this.addressForm.valid ? null : { address: { valid: false } };
  }

  onChange: any = () => {};
  onTouched: any = () => {};

  writeValue(value: AddressFormValues): void {
    if (value) {
      this.addressFormValue = value;
    }

    if (value === null) {
      this.addressForm.reset();
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
