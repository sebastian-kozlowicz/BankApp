import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-bank-transfer',
  templateUrl: './bank-transfer.component.html',
  styleUrls: ['./bank-transfer.component.css']
})
export class BankTransferComponent implements OnInit {

  constructor() { }

  requesterBankAccountId: number;

  ngOnInit(): void {
    this.requesterBankAccountId = history.state.requesterBankAccountId;
  }
}
