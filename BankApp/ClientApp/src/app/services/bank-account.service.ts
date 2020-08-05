import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BankAccountWithCustomerCreation } from "../interfaces/bank-account/with-customer/bank-account-with-customer-creation";
import { BankAccountCreation } from "../interfaces/bank-account/bank-account-creation";

@Injectable({
  providedIn: 'root'
})
export class BankAccountService {

  constructor(private http: HttpClient) { }

  private readonly bankAccountEndpoint = '/api/BankAccounts';

  createBankAccount(bankAccount: BankAccountCreation) {
    return this.http.post(this.bankAccountEndpoint, bankAccount);
  }

  createBankAccountWithCustomer(bankAccountWithCustomer: BankAccountWithCustomerCreation) {
    return this.http.post(this.bankAccountEndpoint + '/CreateWithCustomer', bankAccountWithCustomer);
  }

  getBankAccount(bankAccountId: number) {
    return this.http.get(this.bankAccountEndpoint + '/' + bankAccountId);
  }

  getBankAccounts(applicationUserId: number) {
    return this.http.get(this.bankAccountEndpoint + '/GetAllForUser/' + applicationUserId);
  }
}
