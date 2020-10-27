import { Component } from '@angular/core';
import { FormBuilder, Validators } from "@angular/forms";
import { ToastrService } from "ngx-toastr";
import { AuthService } from "../../services/auth.service";
import { PersonalInformationFormValues } from "../../interfaces/forms/personal-information-form-values";
import { AddressFormValues } from "../../interfaces/forms/address-form-values";
import { UserRole } from '../../enumerators/userRole';
import { Register } from "../../interfaces/auth/register";

@Component({
  selector: 'app-new-user',
  templateUrl: './new-user.component.html',
  styleUrls: ['./new-user.component.css']
})
export class NewUserComponent {

  constructor(private fb: FormBuilder,
    private authService: AuthService,
    private toastr: ToastrService) { }

  UserRole = UserRole;
  USER_ROLES: Array<UserRole> = [UserRole.Administrator, UserRole.Employee, UserRole.Manager];

  get userRole() {
    return this.accountInformationForm.get('userRole');
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

  accountInformationForm = this.fb.group({
    userRole: ['', Validators.required],
  });

  personalInformationForm = this.fb.group({
    personalInformation: [null, Validators.required]
  });

  residentialAddressForm = this.fb.group({
    address: [null, Validators.required]
  });

  submit() {
    let registerModel: Register = {
      user: {
        name: this.personalInformationValue.name,
        surname: this.personalInformationValue.surname,
        email: this.personalInformationValue.email,
        phoneNumber: this.personalInformationValue.phoneNumber.toString(),
        password: "",
      },
      address: {
        country: this.addressValue.country,
        city: this.addressValue.city,
        street: this.addressValue.street,
        houseNumber: this.addressValue.houseNumber,
        apartmentNumber: this.addressValue.apartmentNumber,
        postalCode: this.addressValue.postalCode.toString()
      }
    };
  }
}
