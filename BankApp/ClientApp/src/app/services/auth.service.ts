import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Register } from '../models/register';
import { map } from 'rxjs/operators';

import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private jwtHelper: JwtHelperService) { }

  private readonly administratorRegistrationEndpoint = '/api/auth/register/administrator';
  private readonly customerRegistrationEndpoint = '/api/auth/register/customer';
  private readonly employeeRegistrationEndpoint = '/api/auth/register/employee';
  private readonly managerRegistrationEndpoint = '/api/auth/register/manager';
  private readonly loginEndpoint = '/api/auth/login';

  registerAdministrator(registerModel: Register) {
    return this.http.post(this.administratorRegistrationEndpoint, registerModel);
  }

  registerCustomer(registerModel: Register) {
    return this.http.post(this.customerRegistrationEndpoint, registerModel);
  }

  registerEmployee(registerModel: Register) {
    return this.http.post(this.employeeRegistrationEndpoint, registerModel);
  }

  registerManager(registerModel: Register) {
    return this.http.post(this.managerRegistrationEndpoint, registerModel);
  }

  login(loginModel) {
    return this.http.post(this.loginEndpoint, loginModel)
      .pipe(map(
        (response: any) => {
          if (response && response.token) {
            sessionStorage.setItem('token', response.token);
            return true;
          }
          return false;
        }
      ));
  }

  logout() {
    sessionStorage.removeItem('token');
  }

  isLoggedIn() {
    return this.jwtHelper.getTokenExpirationDate();
  }

  get currentUser() {
    let token = sessionStorage.getItem('token');

    if (!token)
      return null;

    return this.jwtHelper.decodeToken(token);
  }

  isAdministrator() {
    let user = this.currentUser;

    if (user && user.administrator === true)
      return true;

    return false;
  }

  isCustomer() {
    let user = this.currentUser;

    if (user && user.customer === true)
      return true;

    return false;
  }

  isEmployee() {
    let user = this.currentUser;

    if (user && user.employee === true)
      return true;

    return false;
  }

  isManager() {
    let user = this.currentUser;

    if (user && user.manager === true)
      return true;

    return false;
  }
}
