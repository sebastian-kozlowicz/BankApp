import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs/operators';
import { UntypedFormBuilder, Validators } from '@angular/forms';

import { BankAccountService } from '../../services/bank-account.service';
import { AccountType } from '../../enumerators/accountType';
import { Currency } from '../../enumerators/currency';
import { PasswordValidator } from '../../validators/password-validator';
import { PersonalInformationFormValues } from '../../interfaces/forms/personal-information-form-values';
import { AddressFormValues } from '../../interfaces/forms/address-form-values';
import { BankAccountWithCustomerCreation } from
  '../../interfaces/bank-account/with-customer/bank-account-with-customer-creation';
import { ACCOUNT_TYPES } from '../../constants/app-constants';


@Component({
  standalone: false,
  selector: 'app-registration',
  templateUrl: './new-bank-account-with-customer.component.html',
  styleUrls: ['./new-bank-account-with-customer.component.css']
})
export class NewBankAccountWithCustomer {

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal('');
  readonly registrationComplete = signal(false);
  private readonly destroyRef = inject(DestroyRef);

  constructor(private fb: UntypedFormBuilder,
    private bankAccountService: BankAccountService) {
  }

  AccountType = AccountType;
  accountTypes = ACCOUNT_TYPES;

  currencies = [
    { name: 'Polish złoty', code: Currency.Pln },
    { name: 'Euro', code: Currency.Eur },
    { name: 'US dollar', code: Currency.Usd },
    { name: 'Pound sterling', code: Currency.Gbp }
  ];

  get accountType() {
    return this.accountInformationForm.get('accountType');
  }

  get currency() {
    return this.accountInformationForm.get('currency');
  }

  get personalInformation() {
    return this.personalInformationForm.get('personalInformation');
  }

  get personalInformationValue(): PersonalInformationFormValues {
    return this.personalInformation.value;
  }

  get address() {
    return this.residentialAddressForm.get('address');
  }

  get addressValue(): AddressFormValues {
    return this.address.value;
  }

  get password() {
    return this.passwordForm.get('password');
  }

  get confirmPassword() {
    return this.passwordForm.get('confirmPassword');
  }

  accountInformationForm = this.fb.group({
    accountType: ['', Validators.required],
    currency: ['', Validators.required]
  });

  personalInformationForm = this.fb.group({
    personalInformation: [null, Validators.required]
  });

  residentialAddressForm = this.fb.group({
    address: [null, Validators.required]
  });

  passwordForm = this.fb.group({
      password: [
        '', Validators.compose([
          Validators.required,
          PasswordValidator.patternValidator(/\d/, { hasNumber: true }),
          PasswordValidator.patternValidator(/[A-Z]/, { hasCapitalCase: true }),
          PasswordValidator.patternValidator(/[a-z]/, { hasLowerCase: true }),
          PasswordValidator.patternValidator(/[-!$%^&*()_+|~=`{}\[\]:\/;<>?,.@#]/, { hasSpecialCharacter: true }),
          Validators.minLength(8)
        ])
      ],
      confirmPassword: ['', Validators.required]
    },
    {
      validator: PasswordValidator.isPasswordMatch
    });

  register() {
    if (this.isSubmitting() || this.registrationComplete()) {
      return;
    }

    const forms = [this.accountInformationForm, this.personalInformationForm, this.residentialAddressForm, this.passwordForm];
    if (forms.some(form => form.invalid)) {
      forms.forEach(form => form.markAllAsTouched());
      return;
    }

    const registerModel: BankAccountWithCustomerCreation = {
      register: {
        user: {
          name: this.personalInformationValue.name,
          surname: this.personalInformationValue.surname,
          email: this.personalInformationValue.email,
          phoneNumber: this.personalInformationValue.phoneNumber,
          password: this.password.value,
        },
        address: {
          country: this.addressValue.country,
          city: this.addressValue.city,
          street: this.addressValue.street,
          houseNumber: this.addressValue.houseNumber,
          apartmentNumber: this.addressValue.apartmentNumber,
          postalCode: this.addressValue.postalCode
        }
      },
      bankAccount: {
        accountType: this.accountType.value,
        currency: this.currency.value
      }
    };

    this.errorMessage.set('');
    this.isSubmitting.set(true);
    this.bankAccountService.createBankAccountWithCustomerByCustomer(registerModel)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.isSubmitting.set(false))
      ).subscribe({
        next: response => {
          if (response) {
            this.registrationComplete.set(true);
            this.passwordForm.reset();
          } else {
            this.errorMessage.set('We could not confirm your registration. Try signing in before submitting again.');
          }
        },
        error: badRequest => {
          const duplicateEmail = Array.isArray(badRequest.error) &&
            badRequest.error.some(element => element?.code === 'DuplicateUserName' || element?.code === 'DuplicateEmail');
          this.errorMessage.set(duplicateEmail
            ? 'Registration failed. This email may already be registered. Try signing in.'
            : 'Registration failed. Please check your details and try again.');
        }
      });
  }
}
