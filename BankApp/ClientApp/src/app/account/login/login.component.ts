import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../../services/account.service';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private fb: FormBuilder, private accountService: AccountService, private toastr: ToastrService, private router: Router) { }

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
      response => {
        if (response)
          this.router.navigateByUrl('/');
      },
      error => {
        if (error.status == 400)
          this.toastr.error('Invalid login attempt');
      }
    );
  }
}
