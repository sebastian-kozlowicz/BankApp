import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { BankAccountService } from '../../../services/bank-account.service';
import { Router } from '@angular/router';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { BankTransferCreation } from '../../../interfaces/bank-transfer/bank-transfer-creation';
import { BankTransferService } from '../../../services/bank-transfer.service';
import { ToastrService } from 'ngx-toastr';
import { BankAccount } from '../../../interfaces/bank-account/bank-account';
import { Currency } from '../../../enumerators/currency';

@Component({
  standalone: false,
  selector: 'app-bank-transfer',
  templateUrl: './bank-transfer.component.html',
  styleUrls: ['./bank-transfer.component.css']
})
export class BankTransferComponent implements OnInit {

  constructor(private authService: AuthService,
    private fb: UntypedFormBuilder,
    private bankAccountService: BankAccountService,
    private bankTransferService: BankTransferService,
    private router: Router,
    private toastr: ToastrService) {
  }

  requesterBankAccountId: number;
  bankAccount: BankAccount;
  isLoading = true;
  isSubmitting = false;

  get receiverIban() {
    return this.bankTransferForm.get('receiverIban');
  }

  get value() {
    return this.bankTransferForm.get('value');
  }

  get currencyCode(): string {
    return this.bankAccount ? Currency[this.bankAccount.currency]?.toUpperCase() ?? '' : '';
  }

  get maskedIban(): string {
    const iban = this.bankAccount?.iban || this.bankAccount?.ibanSeparated || '';
    return iban.length > 8 ? `${iban.slice(0, 4)} •••• •••• ${iban.slice(-4)}` : iban;
  }

  ngOnInit(): void {
    this.requesterBankAccountId = history.state?.requesterBankAccountId;

    if (!this.requesterBankAccountId) {
      this.router.navigate(['/no-access'], { skipLocationChange: true });
      return;
    }

    const user = this.authService.currentUser;

    this.bankAccountService.getBankAccount(this.requesterBankAccountId).subscribe(
      bankAccount => {
        if (bankAccount.customerId != user.userId) {
          this.router.navigate(['/no-access'], { skipLocationChange: true });
          return;
        }

        this.bankAccount = bankAccount;
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.router.navigate(['/no-access'], { skipLocationChange: true }); // Todo: Add error page
      });
  }

  bankTransferForm = this.fb.group({
    receiverIban: ['', Validators.required],
    value: ['', [Validators.required, Validators.min(0.01)]]
  });

  submit() {
    if (this.bankTransferForm.invalid || this.isSubmitting) {
      this.bankTransferForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const bankTransferModel: BankTransferCreation = {
      requesterBankAccountId: this.requesterBankAccountId,
      receiverIban: this.receiverIban.value,
      value: this.value.value
    };

    this.bankTransferService.createBankTransfer(bankTransferModel).subscribe(
      response => {
        this.isSubmitting = false;
        this.toastr.success('Bank transfer created');
      },
      error => {
        this.isSubmitting = false;
        this.toastr.error('Bank transfer failed.');
      }
    );
  }
}
