import { Component } from '@angular/core';
import { FormBuilder, Validators } from "@angular/forms";
import { ToastrService } from "ngx-toastr";
import { AuthService } from "../../services/auth.service";
import { PersonalInformationFormValues } from "../../interfaces/forms/personal-information-form-values";
import { AddressFormValues } from "../../interfaces/forms/address-form-values";
import { UserRole } from '../../enumerators/userRole';
import { RegisterByAnotherUser } from "../../interfaces/auth/register-by-another-user";

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
  USER_ROLES: Array<UserRole> = [UserRole.Administrator, UserRole.Teller, UserRole.Manager];

  get userRole() {
    return this.accountInformationForm.get('userRole');
  }
  get userRoleValue(): UserRole {
    return this.userRole.value;
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
    let registerModel: RegisterByAnotherUser = {
      user: {
        name: this.personalInformationValue.name,
        surname: this.personalInformationValue.surname,
        email: this.personalInformationValue.email,
        phoneNumber: this.personalInformationValue.phoneNumber.toString()
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

    let serviceMethodName; 

    if (this.userRoleValue === UserRole.Administrator) {
      serviceMethodName = "registerAdministrator";
    }
    else if (this.userRoleValue === UserRole.Customer) {
      serviceMethodName = "registerCustomer";
    }
    else if (this.userRoleValue === UserRole.Teller) {
      serviceMethodName = "registerTeller";
    }
    else if (this.userRoleValue === UserRole.Manager) {
      serviceMethodName = "registerManager";
    }

    this.authService[serviceMethodName](registerModel).subscribe(
      response => {
        this.toastr.success('User created');
      },
      error => {
        this.toastr.error('User not created');
      }
    );
  }
}
