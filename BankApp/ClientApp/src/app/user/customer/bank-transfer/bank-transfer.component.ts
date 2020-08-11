import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { BankAccountService } from '../../../services/bank-account.service';
import { Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-bank-transfer',
  templateUrl: './bank-transfer.component.html',
  styleUrls: ['./bank-transfer.component.css']
})
export class BankTransferComponent implements OnInit {

  constructor(private authService: AuthService,
    private fb: FormBuilder,
    private bankAccountService: BankAccountService,
    private router: Router) { }

  requesterBankAccountId: number;

  get receiverIban() {
    return this.bankTransferForm.get('receiverIban');
  }
  get value() {
    return this.bankTransferForm.get('value');
  }

  ngOnInit(): void {
    this.requesterBankAccountId = history.state.requesterBankAccountId;

    let user = this.authService.currentUser;

    this.bankAccountService.getBankAccount(this.requesterBankAccountId).subscribe(
      bankAccount => {
        if (bankAccount.applicationUserId != user.userId)
          this.router.navigate(['/no-access'], { skipLocationChange: true })
      },
      error => {
        this.router.navigate(['/no-access'], { skipLocationChange: true })
      });
  }

    bankTransferForm = this.fb.group({
      receiverIban: ['', Validators.required],
      value: ['', Validators.required]
    });
}
