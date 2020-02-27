import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private fb: FormBuilder) { }

  get email() {
    return this.loginFormModel.get('email');
  }
  get password() {
    return this.loginFormModel.get('password');
  }

  loginFormModel = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });


  ngOnInit(): void {
  }

}
