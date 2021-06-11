import { Component, forwardRef } from '@angular/core';
import { FormBuilder, Validators, ControlValueAccessor, NG_VALUE_ACCESSOR, NG_VALIDATORS, FormControl } from
  '@angular/forms';
import { NumberLimitValidator } from '../../validators/number-limit-validator';
import { Subscription } from 'rxjs';
import { PersonalInformationFormValues } from '../../interfaces/forms/personal-information-form-values';

@Component({
  selector: 'app-personal-information-form',
  templateUrl: './personal-information-form.component.html',
  styleUrls: ['./personal-information-form.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => PersonalInformationFormComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => PersonalInformationFormComponent),
      multi: true
    }
  ]
})
export class PersonalInformationFormComponent implements ControlValueAccessor {

  constructor(private fb: FormBuilder) {
    this.subscriptions.push(
      this.personalInformationForm.valueChanges.subscribe(value => {
        this.onChange(value);
        this.onTouched();
      })
    );
  }

  subscriptions: Subscription[] = [];

  get personalInformationFormValue(): PersonalInformationFormValues {
    return this.personalInformationForm.value;
  }

  set personalInformationFormValue(value: PersonalInformationFormValues) {
    this.personalInformationForm.setValue(value);
    this.onChange(value);
    this.onTouched();
  }

  get name() {
    return this.personalInformationForm.get('name');
  }

  get surname() {
    return this.personalInformationForm.get('surname');
  }

  get phoneNumber() {
    return this.personalInformationForm.get('phoneNumber');
  }

  get email() {
    return this.personalInformationForm.get('email');
  }

  personalInformationForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
    surname: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
    phoneNumber: [
      '', [Validators.required, NumberLimitValidator.limitValidator(100000, 100000000000, { invalidLimit: true })]
    ],
    email: ['', [Validators.required, Validators.email]]
  });

  validate(_: FormControl) {
    return this.personalInformationForm.valid ? null : { personalInformation: { valid: false } };
  }

  onChange: any = () => {};
  onTouched: any = () => {};

  writeValue(value: PersonalInformationFormValues): void {
    if (value) {
      this.personalInformationFormValue = value;
    }

    if (value === null) {
      this.personalInformationForm.reset();
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
