import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BankTransferCreation } from '../interfaces/bank-transfer/bank-transfer-creation';

@Injectable({
  providedIn: 'root'
})
export class BankTransferService {

  constructor(private http: HttpClient) { }

  private readonly bankTransfersEndpoint = '/api/banktransfers';

  createBankTransfer(bankTransfer: BankTransferCreation) {
    return this.http.post(this.bankTransfersEndpoint, bankTransfer);
  }
}
