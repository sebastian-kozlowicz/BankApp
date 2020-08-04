import { Component, OnInit } from '@angular/core';
import { BankAccountService } from '../../../services/bank-account.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-bank-accounts',
  templateUrl: './bank-accounts.component.html',
  styleUrls: ['./bank-accounts.component.css']
})
export class BankAccountsComponent implements OnInit {

  constructor(private bankAccountService: BankAccountService,
    private authService: AuthService) { }

  bankAccounts;

  ngOnInit(): void {
    this.bankAccountService.getBankAccounts(this.authService.currentUser.userId)
      .subscribe(bankAccounts => {
        this.bankAccounts = bankAccounts
      })
  }

}
