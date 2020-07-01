import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class BankAccountService {

  constructor(private http: HttpClient) { }

  private readonly bankAccountEndpoint = '/api/bankaccounts';

  createBankAccount(bankAccount) {
    return this.http.post(this.bankAccountEndpoint, bankAccount);
  }

  createBankAccountWithCustomer(bankAccountWithCustomer) {
    return this.http.post(this.bankAccountEndpoint + '/create-with-customer', bankAccountWithCustomer);
  }
}
