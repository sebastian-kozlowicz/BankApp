import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { BankAccountService } from '../../../services/bank-account.service';
import { Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { BankTransferCreation } from '../../../interfaces/bank-transfer/bank-transfer-creation';
import { BankTransferService } from '../../../services/bank-transfer.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-bank-transfer',
  templateUrl: './bank-transfer.component.html',
  styleUrls: ['./bank-transfer.component.css']
})
export class BankTransferComponent implements OnInit {

  constructor(private authService: AuthService,
    private fb: FormBuilder,
    private bankAccountService: BankAccountService,
    private bankTransferService: BankTransferService,
    private router: Router,
    private toastr: ToastrService) {
  }

  requesterBankAccountId: number;

  get receiverIban() {
    return this.bankTransferForm.get('receiverIban');
  }

  get value() {
    return this.bankTransferForm.get('value');
  }

  ngOnInit(): void {
    this.requesterBankAccountId = history.state.requesterBankAccountId;

    if (!this.requesterBankAccountId) {
      this.router.navigate(['/no-access'], { skipLocationChange: true });
      return;
    }

    const user = this.authService.currentUser;

    this.bankAccountService.getBankAccount(this.requesterBankAccountId).subscribe(
      bankAccount => {
        if (bankAccount.customerId != user.userId)
          this.router.navigate(['/no-access'], { skipLocationChange: true });
      },
      error => {
        this.router.navigate(['/no-access'], { skipLocationChange: true }); // Todo: Add error page
      });
  }

  bankTransferForm = this.fb.group({
    receiverIban: ['', Validators.required],
    value: ['', Validators.required]
  });

  submit() {
    const bankTransferModel: BankTransferCreation = {
      requesterBankAccountId: this.requesterBankAccountId,
      receiverIban: this.receiverIban.value,
      value: this.value.value
    };

    this.bankTransferService.createBankTransfer(bankTransferModel).subscribe(
      response => {
        this.toastr.success('Bank transfer created');
      },
      error => {
        this.toastr.error('Bank transfer failed.');
      }
    );
  }
}
