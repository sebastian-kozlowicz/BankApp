import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BankAccountWithCustomerCreation } from "../interfaces/bank-account/with-customer/bank-account-with-customer-creation";
import { BankAccountCreation } from "../interfaces/bank-account/bank-account-creation";
import { BankAccount } from '../interfaces/bank-account/bank-account';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BankAccountService {

  constructor(private http: HttpClient) { }

  private readonly bankAccountEndpoint = '/api/BankAccounts';

  createBankAccount(bankAccount: BankAccountCreation): Observable<BankAccount>{
    return this.http.post<BankAccount>(this.bankAccountEndpoint, bankAccount);
  }

  createBankAccountWithCustomerByCustomer(bankAccountWithCustomer: BankAccountWithCustomerCreation) :Observable<BankAccount> {
    return this.http.post<BankAccount>(this.bankAccountEndpoint + '/CreateWithCustomerByCustomer', bankAccountWithCustomer);
  }

  getBankAccount(bankAccountId: number): Observable<BankAccount> {
    return this.http.get<BankAccount>(this.bankAccountEndpoint + '/' + bankAccountId);
  }

  getBankAccounts(applicationUserId: number): Observable<BankAccount[]> {
    return this.http.get<BankAccount[]>(this.bankAccountEndpoint + '/GetAllForUser/' + applicationUserId);
  }
}
