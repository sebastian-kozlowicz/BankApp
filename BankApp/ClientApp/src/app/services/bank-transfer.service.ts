import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class BankTransferService {

  constructor(private http: HttpClient) { }

  private readonly bankBankTransfer = '/api/banktransfers';

  createBankTransfer(bankTransfer) {
    return this.http.post(this.bankBankTransfer, bankTransfer);
  }
}
