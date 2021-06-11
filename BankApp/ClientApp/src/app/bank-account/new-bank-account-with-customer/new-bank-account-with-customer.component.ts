import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
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
  selector: 'app-registration',
  templateUrl: './new-bank-account-with-customer.component.html',
  styleUrls: ['./new-bank-account-with-customer.component.css']
})
export class NewBankAccountWithCustomer {

  constructor(private fb: FormBuilder,
    private bankAccountService: BankAccountService,
    private toastr: ToastrService) {
  }

  AccountType = AccountType;
  accountTypes = ACCOUNT_TYPES;

  currencies = [
    { name: 'Polish złoty', code: Currency.Pln },
    { name: 'Euro', code: Currency.Eur },
    { name: 'US dollar', code: Currency.Usd },
    { name: 'Pound sterling', code: Currency.Gbp }
  ];

  get getPasswordErrorCount() {
    let errorCount = 0;

    if (PasswordValidator.isErrorOccur(this.password, 'required'))
      errorCount++;
    if (PasswordValidator.isErrorOccur(this.password, 'hasNumber'))
      errorCount++;
    if (PasswordValidator.isErrorOccur(this.password, 'hasCapitalCase'))
      errorCount++;
    if (PasswordValidator.isErrorOccur(this.password, 'hasLowerCase'))
      errorCount++;
    if (PasswordValidator.isErrorOccur(this.password, 'hasSpecialCharacter'))
      errorCount++;
    if (PasswordValidator.isErrorOccur(this.password, 'minlength'))
      errorCount++;

    return errorCount;
  }

  setErrorClasses() {
    const errorCount = this.getPasswordErrorCount;

    return {
      'one-error': errorCount === 1,
      'two-errors': errorCount === 2,
      'three-errors': errorCount === 3,
      'four-errors': errorCount === 4
    };
  }

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

    this.bankAccountService.createBankAccountWithCustomerByCustomer(registerModel).subscribe(
      response => {
        if (response)
          this.toastr.success('New user created!', 'Registration successful.');
      },
      badRequest => {
        if (Array.isArray(badRequest.error))
          badRequest.error.forEach(element => {
            if (element.code === 'DuplicateUserName')
              this.toastr.error('Username is already taken', 'Registration failed.');
          });
        else
          this.toastr.error('Registration failed.');
      }
    );
  }
}
