import { Component, signal } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { AccountType } from '../../../enumerators/accountType';
import { Currency } from '../../../enumerators/currency';
import { BankAccountService } from '../../../services/bank-account.service';

@Component({
  standalone: false,
  selector: 'app-customer-new-bank-account',
  templateUrl: './customer-new-bank-account.component.html',
  styleUrls: ['./customer-new-bank-account.component.css']
})
export class CustomerNewBankAccountComponent {
  readonly isSubmitting = signal(false);
  readonly accountTypes = [
    { value: AccountType.Checking, name: 'Checking account', description: 'For everyday payments and transfers.' },
    { value: AccountType.Savings, name: 'Savings account', description: 'A separate place for your savings.' }
  ];
  readonly currencies = [
    { value: Currency.Pln, code: 'PLN', name: 'Polish złoty' },
    { value: Currency.Eur, code: 'EUR', name: 'Euro' },
    { value: Currency.Usd, code: 'USD', name: 'US dollar' },
    { value: Currency.Gbp, code: 'GBP', name: 'Pound sterling' }
  ];

  readonly accountForm = this.formBuilder.group({
    accountType: [null, Validators.required],
    currency: [null, Validators.required]
  });

  constructor(
    private formBuilder: UntypedFormBuilder,
    private bankAccountService: BankAccountService,
    private router: Router,
    private toastr: ToastrService) {
  }

  createAccount(): void {
    if (this.accountForm.invalid || this.isSubmitting()) {
      this.accountForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.bankAccountService.createBankAccountForCurrentCustomer(this.accountForm.value)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          this.toastr.success('Your new bank account is ready.');
          this.router.navigate(['/customer/profile']);
        },
        error: () => this.toastr.error('We could not create your account. Please try again.')
      });
  }
}
