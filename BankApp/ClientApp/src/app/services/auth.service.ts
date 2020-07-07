import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Register } from '../models/auth/register';
import { map } from 'rxjs/operators';

import { JwtHelperService } from '@auth0/angular-jwt';
import { JwtToken } from '../models/auth/jwtToken';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private jwtHelper: JwtHelperService) { }

  private readonly administratorsEndpoint = '/api/administrators';
  private readonly customersEndpoint = '/api/customers';
  private readonly employeesEndpoint = '/api/employees';
  private readonly managersEndpoint = '/api/managers';
  private readonly loginEndpoint = '/api/auth/login';

  registerAdministrator(registerModel: Register) {
    return this.http.post(this.administratorsEndpoint, registerModel);
  }

  registerCustomer(registerModel: Register) {
    return this.http.post(this.customersEndpoint, registerModel);
  }

  registerEmployee(registerModel: Register) {
    return this.http.post(this.employeesEndpoint, registerModel);
  }

  registerManager(registerModel: Register) {
    return this.http.post(this.managersEndpoint, registerModel);
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

  get currentUser():JwtToken {
    let token = sessionStorage.getItem('token');

    if (!token)
      return null;
    console.log(this.jwtHelper.decodeToken(token));
    return this.jwtHelper.decodeToken(token);
  }

  isAdministrator() {
    let user = this.currentUser;

    if (user && user.role.includes('Administrator'))
      return true;

    return false;
  }

  isCustomer() {
    let user = this.currentUser;

    if (user && user.role.includes('Customer'))
      return true;

    return false;
  }

  isEmployee() {
    let user = this.currentUser;

    if (user && user.role.includes('Employee'))
      return true;

    return false;
  }

  isManager() {
    let user = this.currentUser;

    if (user && user.role.includes('Manager'))
      return true;

    return false;
  }
}
