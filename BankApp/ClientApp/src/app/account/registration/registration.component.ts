import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { Register } from '../../models/register';
import { PasswordValidator } from '../../validators/password-validator';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {

  constructor(private fb: FormBuilder, private accountService: AccountService, private toastr: ToastrService) { }

  registerFormModel = this.fb.group({
    name: ['', Validators.required],
    surname: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.compose([
      Validators.required,
      PasswordValidator.patternValidator(/\d/, { hasNumber: true }),
      PasswordValidator.patternValidator(/[A-Z]/, { hasCapitalCase: true }),
      PasswordValidator.patternValidator(/[a-z]/, { hasLowerCase: true }),
      PasswordValidator.patternValidator(/[-!$%^&*()_+|~=`{}\[\]:\/;<>?,.@#]/, { hasSpecialCharacter: true }),
      Validators.minLength(8)])
    ],
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

  onSubmit() {
    let registerModel: Register = {
      name: this.registerFormModel.controls['name'].value,
      surname: this.registerFormModel.controls['surname'].value,
      email: this.registerFormModel.controls['email'].value,
      password: this.registerFormModel.controls['password'].value,
    };

    this.accountService.register(registerModel).subscribe(
      response => {
        if (response) {
          this.toastr.success('New user created!', 'Registration successful.');
        }
      },
      badRequest => {
        if (Array.isArray(badRequest.error))
          badRequest.error.forEach(element => {
            if (element.code == 'DuplicateUserName')
              this.toastr.error('Username is already taken', 'Registration failed.');
          });
        else
          this.toastr.error('Registration failed.');
      }
    );
  }
}
