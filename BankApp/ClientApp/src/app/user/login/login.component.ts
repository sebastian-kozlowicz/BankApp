import { Component, OnInit, signal } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';
import { finalize } from 'rxjs/operators';

@Component({
  standalone: false,
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  readonly isSubmitting = signal(false);
  readonly passwordVisible = signal(false);

  constructor(private fb: UntypedFormBuilder,
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

  togglePasswordVisibility(): void {
    this.passwordVisible.update(visible => !visible);
  }

  onSubmit() {
    if (this.loginFormModel.invalid || this.isSubmitting()) {
      this.loginFormModel.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.authService.login(this.loginFormModel.value)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: response => {
          if (!response) {
            this.toastr.error('Invalid email or password');
            return;
          }

          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
          const destination = this.authService.isCustomer()
            ? returnUrl || '/customer/profile'
            : returnUrl || '/';

          this.router.navigate([destination]);
        },
        error: () => this.toastr.error('Invalid email or password')
      });
  }
}
