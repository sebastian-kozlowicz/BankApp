import { Component } from '@angular/core';
import { FormBuilder, Validators } from "@angular/forms";
import { ToastrService } from "ngx-toastr";
import { AuthService } from "../../services/auth.service";
import { PersonalInformationFormValues } from "../../interfaces/forms/personal-information-form-values";
import { AddressFormValues } from "../../interfaces/forms/address-form-values";

@Component({
  selector: 'app-new-user',
  templateUrl: './new-user.component.html',
  styleUrls: ['./new-user.component.css']
})
export class NewUserComponent {

  constructor(private fb: FormBuilder,
    private authService: AuthService,
    private toastr: ToastrService) { }

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

  personalInformationForm = this.fb.group({
    personalInformation: [null, Validators.required]
  });

  residentialAddressForm = this.fb.group({
    address: [null, Validators.required]
  });
}
