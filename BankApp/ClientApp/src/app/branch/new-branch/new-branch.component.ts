import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { BranchService } from '../../services/branch.service';
import { ToastrService } from 'ngx-toastr';
import { BranchWithAddressCreation } from '../../interfaces/branch/with-address/branch-with-address-creation';
import { AddressFormValues } from '../../interfaces/forms/address-form-values';

@Component({
  selector: 'app-new-branch',
  templateUrl: './new-branch.component.html',
  styleUrls: ['./new-branch.component.css']
})
export class NewBranchComponent implements OnInit {

  constructor(private fb: FormBuilder,
    private branchService: BranchService,
    private toastr: ToastrService) {
  }

  get branchCode() {
    return this.branchForm.get('branchCode');
  }

  get address() {
    return this.branchForm.get('address');
  }

  get addressValue(): AddressFormValues {
    return this.address.value;
  }

  ngOnInit(): void {
  }

  branchForm = this.fb.group({
    branchCode: ['', Validators.required],
    address: [null, Validators.required]
  });

  submit() {
    const branchModel: BranchWithAddressCreation = {
      branch: {
        branchCode: this.branchCode.value
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

    this.branchService.createBranch(branchModel).subscribe(
      response => {
        this.toastr.success('New branch created!');
      },
      badRequest => {
        console.log(badRequest);
        if (Array.isArray(badRequest.error.BranchCode))
          badRequest.error.BranchCode.forEach(element => {
            if (element == 'Branch code is already in use.')
              this.toastr.error('Branch code is already in use.', 'Branch creation failed.');
          });
        else if (Array.isArray(badRequest.error.errors['Branch.BranchCode']))
          badRequest.error.errors['Branch.BranchCode'].forEach(element => {
            if (element == 'String length can only contain digits and must be of length 3 characters.')
              this.toastr.error('String length can only contain digits and must be of length 3 characters.',
                'Branch creation failed.');
          });
        else
          this.toastr.error('Branch creation failed');
      }
    );
  }
}
