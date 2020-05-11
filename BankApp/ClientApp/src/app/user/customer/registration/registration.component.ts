import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup, AbstractControl } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from "../../../services/auth.service";
import { PasswordValidator } from "../../../validators/password-validator";
import { Register } from "../../../models/register";
import { Currency } from '../../../enumerators/currency';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class CustomerRegistrationComponent implements OnInit {

  constructor(private fb: FormBuilder, private authService: AuthService, private toastr: ToastrService) { }

  countries = ["Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua &amp; Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia", "Bosnia &amp; Herzegovina", "Botswana", "Brazil", "British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Cape Verde", "Cayman Islands", "Chad", "Chile", "China", "Colombia", "Congo", "Cook Islands", "Costa Rica", "Cote D Ivoire", "Croatia", "Cruise Ship", "Cuba", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Estonia", "Ethiopia", "Falkland Islands", "Faroe Islands", "Fiji", "Finland", "France", "French Polynesia", "French West Indies", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea Bissau", "Guyana", "Haiti", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kuwait", "Kyrgyz Republic", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macau", "Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Mauritania", "Mauritius", "Mexico", "Moldova", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Namibia", "Nepal", "Netherlands", "Netherlands Antilles", "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "Norway", "Oman", "Pakistan", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Puerto Rico", "Qatar", "Reunion", "Romania", "Russia", "Rwanda", "Saint Pierre &amp; Miquelon", "Samoa", "San Marino", "Satellite", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "South Africa", "South Korea", "Spain", "Sri Lanka", "St Kitts &amp; Nevis", "St Lucia", "St Vincent", "St. Lucia", "Sudan", "Suriname", "Swaziland", "Sweden", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Timor L'Este", "Togo", "Tonga", "Trinidad &amp; Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks &amp; Caicos", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "Uruguay", "Uzbekistan", "Venezuela", "Vietnam", "Virgin Islands (US)", "Yemen", "Zambia", "Zimbabwe"];
  currencies = [
    { name: "Polish złoty", code: Currency.Pln },
    { name: "Euro", code: Currency.Eur },
    { name: "US dollar", code: Currency.Usd },
    { name: "Pound sterling", code: Currency.Gbp }
  ];

  get getPasswordErrorCount() {
    let errorCount = 0;

    if (this.isErrorOccur(this.password, "required"))
      errorCount++;
    if (this.isErrorOccur(this.password, "hasNumber"))
      errorCount++;
    if (this.isErrorOccur(this.password, "hasCapitalCase"))
      errorCount++;
    if (this.isErrorOccur(this.password, "hasLowerCase"))
      errorCount++;
    if (this.isErrorOccur(this.password, "hasSpecialCharacter"))
      errorCount++;
    if (this.isErrorOccur(this.password, "minlength"))
      errorCount++;

    return errorCount;
  }

  setErrorClasses() {
    let errorCount = this.getPasswordErrorCount;

    return {
      'one-error': errorCount == 1,
      'two-errors': errorCount == 2,
      'three-errors': errorCount == 3,
      'four-errors': errorCount == 4
    };
  }

  get accountType() {
    return this.accountInformationForm.get('accountType');
  }
  get currency() {
    return this.accountInformationForm.get('currency');
  }
  get name() {
    return this.personalInformationForm.get('name');
  }
  get surname() {
    return this.personalInformationForm.get('surname');
  }
  get email() {
    return this.personalInformationForm.get('email');
  }
  get street() {
    return this.residentialAddressForm.get('street');
  }
  get houseNumber() {
    return this.residentialAddressForm.get('houseNumber');
  }
  get apartmentNumber() {
    return this.residentialAddressForm.get('apartmentNumber');
  }
  get postalCode() {
    return this.residentialAddressForm.get('postalCode');
  }
  get city() {
    return this.residentialAddressForm.get('city');
  }
  get country() {
    return this.residentialAddressForm.get('country');
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
    name: ['', Validators.required],
    surname: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]]
  });

  residentialAddressForm = this.fb.group({
    street: ['', Validators.required],
    houseNumber: ['', Validators.required],
    apartmentNumber: ['', Validators.required],
    postalCode: ['', Validators.required],
    city: ['', Validators.required],
    country: ['', Validators.required]
  });

  passwordForm = this.fb.group({
    password: ['', Validators.compose([
      Validators.required,
      PasswordValidator.patternValidator(/\d/, { hasNumber: true }),
      PasswordValidator.patternValidator(/[A-Z]/, { hasCapitalCase: true }),
      PasswordValidator.patternValidator(/[a-z]/, { hasLowerCase: true }),
      PasswordValidator.patternValidator(/[-!$%^&*()_+|~=`{}\[\]:\/;<>?,.@#]/, { hasSpecialCharacter: true }),
      Validators.minLength(8)])
    ],
    confirmPassword: ['', Validators.required]
  }, {
    validator: this.isPasswordMatch
  });

  isErrorOccur(ac: AbstractControl, error) {
    return ac.errors?.[error];
  }

  isPasswordMatch(fg: FormGroup) {
    let password = fg.controls['password'];
    let confirmPassword = fg.controls['confirmPassword'];

    if (confirmPassword.errors == null) {
      if (password.value == confirmPassword.value)
        confirmPassword.setErrors(null);
      else
        confirmPassword.setErrors({ passwordMismatch: true })
    }
  }

  ngOnInit(): void {
  }

  register() {
    let registerModel: Register = {
      name: this.name.value,
      surname: this.surname.value,
      email: this.email.value,
      password: this.password.value,
    };

    this.authService.registerCustomer(registerModel).subscribe(
      response => {
        if (response) {
          this.toastr.success('New user created!', 'Registration successful.');
        }
      },
      badRequest => {
        if (Array.isArray(badRequest.error))
          badRequest.error.forEach(element => {
            if (element.code == 'DuplicateUserName')
              this.toastr.error('Username is already taken', 'Registration failed.');
          });
        else
          this.toastr.error('Registration failed.');
      }
    );
  }
}
