import { Component, OnInit, signal } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { BankAccountService } from '../../../services/bank-account.service';
import { BankAccount } from '../../../interfaces/bank-account/bank-account';

@Component({
  standalone: false,
  selector: 'app-customer-profile',
  templateUrl: './customer-profile.component.html',
  styleUrls: ['./customer-profile.component.css']
})
export class CustomerProfileComponent implements OnInit {

  constructor(private bankAccountService: BankAccountService,
    private authService: AuthService) {
  }

  readonly bankAccounts = signal<BankAccount[]>([]);
  readonly isLoading = signal(true);
  readonly errorMessage = signal('');

  ngOnInit(): void {
    const currentUser = this.authService.currentUser;

    if (!currentUser) {
      this.isLoading.set(false);
      this.errorMessage.set('User session is not available.');
      return;
    }

    this.bankAccountService.getBankAccounts(currentUser.userId)
      .subscribe({
        next: bankAccounts => {
          this.bankAccounts.set(bankAccounts ?? []);
          this.isLoading.set(false);
        },
        error: error => {
          this.isLoading.set(false);
          if (error.status === 404) {
            this.bankAccounts.set([]);
            return;
          }

          this.errorMessage.set('Could not load bank accounts');
        }
      });
  }
}
