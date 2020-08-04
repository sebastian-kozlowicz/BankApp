import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bank-account-summary',
  templateUrl: './bank-account-summary.component.html',
  styleUrls: ['./bank-account-summary.component.css']
})
export class BankAccountSummaryComponent implements OnInit {

  constructor() { }

  @Input() bankAccount;

  ngOnInit(): void {
  }

}
