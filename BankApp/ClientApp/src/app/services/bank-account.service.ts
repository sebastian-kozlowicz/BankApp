import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BankAccountService {

  constructor(private http: HttpClient) { }

  private readonly bankAccountEndpoint = '/api/bankaccounts';

  createBankAccount(bankAccount) {
    return this.http.post(this.bankAccountEndpoint, bankAccount)
      .pipe(map(
        () => {
          return true;
        }
      ),
        catchError(error => {
          return throwError(error);
        }));
  }
}
