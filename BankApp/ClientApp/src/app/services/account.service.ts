import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Register } from '../models/register';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient) { }

  private readonly registrationEndpoint = '/api/account/register';

  register(registerModel: Register) {
    return this.http.post(this.registrationEndpoint, registerModel);
  }
}
