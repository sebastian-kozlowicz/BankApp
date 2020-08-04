import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { BankAccountService } from '../../../services/bank-account.service';

@Component({
  selector: 'app-customer-profile',
  templateUrl: './customer-profile.component.html',
  styleUrls: ['./customer-profile.component.css']
})
export class CustomerProfileComponent implements OnInit {

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
