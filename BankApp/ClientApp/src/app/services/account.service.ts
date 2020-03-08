import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Register } from '../models/register';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';


@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient, private jwtHelper: JwtHelperService) { }

  private readonly registrationEndpoint = '/api/account/register';
  private readonly loginEndpoint = '/api/account/login';

  errorHandler(error: HttpErrorResponse) {
    return throwError(error);
  }

  register(registerModel: Register) {
    return this.http.post(this.registrationEndpoint, registerModel)
      .pipe(map(
        () => {
          return true;
        }
      ), catchError(error => {
        return this.errorHandler(error)
      }))
  }

  login(loginModel) {
    return this.http.post(this.loginEndpoint, loginModel)
      .pipe(map(
        (response: any) => {
          if (response && response.token) {
            localStorage.setItem('token', response.token);
            return true;
          }
          return false;
        }
      ), catchError(error => {
        return this.errorHandler(error)
      }))
  }

  logout() {
    localStorage.removeItem('token');
  }

  isLoggedIn() {
    return this.jwtHelper.getTokenExpirationDate();
  }
}
