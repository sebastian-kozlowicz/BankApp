import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { JwtHelperService } from '@auth0/angular-jwt';
import { JwtToken } from '../interfaces/auth/jwtToken';
import { UserRole } from '../enumerators/userRole';
import { Login } from '../interfaces/auth/login';
import { RegisterByAnotherUser } from "../interfaces/auth/register-by-another-user";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private jwtHelper: JwtHelperService) { }

  private readonly administratorsEndpoint = '/api/administrators';
  private readonly customersEndpoint = '/api/customers';
  private readonly tellersEndpoint = '/api/tellers';
  private readonly managersEndpoint = '/api/managers';
  private readonly loginEndpoint = '/api/auth/login';

  registerAdministrator(registerModel: RegisterByAnotherUser) {
    return this.http.post(this.administratorsEndpoint, registerModel);
  }

  registerCustomer(registerModel: RegisterByAnotherUser) {
    return this.http.post(this.customersEndpoint, registerModel);
  }

  registerTeller(registerModel: RegisterByAnotherUser) {
    return this.http.post(this.tellersEndpoint, registerModel);
  }

  registerManager(registerModel: RegisterByAnotherUser) {
    return this.http.post(this.managersEndpoint, registerModel);
  }

  login(loginModel: Login) {
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

  get currentUser(): JwtToken {
    let token = sessionStorage.getItem('token');

    if (!token)
      return null;

    return this.jwtHelper.decodeToken(token);
  }

  isAdministrator() {
    let user = this.currentUser;

    if (user && user.role.includes(UserRole[UserRole.Administrator]))
      return true;

    return false;
  }

  isCustomer() {
    let user = this.currentUser;

    if (user && user.role.includes(UserRole[UserRole.Customer]))
      return true;

    return false;
  }

  isEmployee() {
    let user = this.currentUser;

    if (user && user.role.includes(UserRole[UserRole.Teller]))
      return true;

    return false;
  }

  isManager() {
    let user = this.currentUser;

    if (user && user.role.includes(UserRole[UserRole.Manager]))
      return true;

    return false;
  }
}
