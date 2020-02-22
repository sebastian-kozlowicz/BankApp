import { Injectable } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private fb: FormBuilder) { }

  registerModel = this.fb.group({
    name: ['', Validators.required],
    surname: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    confirmPassword: ['', Validators.required]
  }, {
    validator: this.IsPasswordMatch
  });

  IsPasswordMatch(fg: FormGroup) {
    let password = fg.controls['password'];
    let confirmPassword = fg.controls['confirmPassword'];

    if (confirmPassword.errors == null) {
      if (password.value == confirmPassword.value)
        confirmPassword.setErrors(null);
      else
        confirmPassword.setErrors({ passwordMismatch: true })
    }
  }
}
