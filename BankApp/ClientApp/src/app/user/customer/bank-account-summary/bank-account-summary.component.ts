import { Component, Input } from '@angular/core';
import { AccountType } from '../../../enumerators/accountType';
import { Currency } from '../../../enumerators/currency';
import { BankAccount } from '../../../interfaces/bank-account/bank-account';

@Component({
  standalone: false,
  selector: 'app-bank-account-summary',
  templateUrl: './bank-account-summary.component.html',
  styleUrls: ['./bank-account-summary.component.css']
})
export class BankAccountSummaryComponent {
  @Input({ required: true }) bankAccount!: BankAccount;

  get accountTypeLabel(): string {
    return this.bankAccount.accountType === AccountType.Savings ? 'Savings account' : 'Checking account';
  }

  get currencyCode(): string {
    return Currency[this.bankAccount.currency]?.toUpperCase() ?? '';
  }

  get maskedIban(): string {
    const iban = this.bankAccount.iban || this.bankAccount.ibanSeparated || '';
    return iban.length > 8 ? `${iban.slice(0, 4)} •••• •••• ${iban.slice(-4)}` : iban;
  }

}
