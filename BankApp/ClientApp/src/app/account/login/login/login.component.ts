import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../../../services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private fb: FormBuilder, private accountService: AccountService, private router: Router) { }

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

  onSubmit() {
    this.accountService.login(this.loginFormModel.value).subscribe(
      (response: any) => {
        if (response) {
          localStorage.setItem("token", response.token);
          this.router.navigateByUrl('/');
        }
      },
      error => {
        console.log(error);
      }
    );
  }

}
