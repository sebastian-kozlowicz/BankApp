import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private fb: FormBuilder,
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router,
    private route: ActivatedRoute) {
  }

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
        if (response) {
          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
          let isCustomer = this.authService.isCustomer;

          if (isCustomer)
            this.router.navigate([returnUrl || '/customer/profile']);
          else
            this.router.navigate([returnUrl || '/']);
        }
      },
      error => {
        if (error.status == 400)
          this.toastr.error('Invalid login attempt');
      }
    );
  }
}
