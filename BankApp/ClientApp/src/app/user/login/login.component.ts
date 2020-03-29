import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private fb: FormBuilder, private authService: AuthService, private toastr: ToastrService, private router: Router) { }

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
    this.authService.login(this.loginFormModel.value).subscribe(
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
