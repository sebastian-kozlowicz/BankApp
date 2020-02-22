import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {

  constructor(private fb: FormBuilder) { }

   registerModel = this.fb.group({
      name: ['', Validators.required],
      surname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    }, {
      validator: this.isPasswordMatch
    });

   isPasswordMatch(fg: FormGroup) {
    let password = fg.controls['password'];
    let confirmPassword = fg.controls['confirmPassword'];

    if (confirmPassword.errors == null) {
      if (password.value == confirmPassword.value)
        confirmPassword.setErrors(null);
      else
        confirmPassword.setErrors({ passwordMismatch: true })
    }
  }

  ngOnInit(): void {
  }

}
