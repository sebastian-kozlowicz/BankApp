import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { BranchService } from "../../services/branch.service";
import { ToastrService } from 'ngx-toastr';
import { BranchWithAddressCreation } from "../../interfaces/branch/with-address/branch-with-address-creation";

@Component({
  selector: 'app-new-branch',
  templateUrl: './new-branch.component.html',
  styleUrls: ['./new-branch.component.css']
})
export class NewBranchComponent implements OnInit {

  constructor(private fb: FormBuilder,
    private branchService: BranchService,
    private toastr: ToastrService) { }

  get branchCode() {
    return this.branchForm.get('branchCode');
  }

  get address() {
    return this.branchForm.get('address');
  }

  ngOnInit(): void {
  }

  branchForm = this.fb.group({
    branchCode: ['', Validators.required],
    address: [null, Validators.required]
  });

  submit() {
    let branchModel: BranchWithAddressCreation = {
      branch: {
        branchCode: this.branchCode.value
      },
      address: {
        country: this.address.value.country,
        city: this.address.value.city,
        street: this.address.value.street,
        houseNumber: this.address.value.houseNumber,
        apartmentNumber: this.address.value.apartmentNumber,
        postalCode: this.address.value.postalCode.toString()
      }
    };

    this.branchService.createBranch(branchModel).subscribe(
      response => {
        console.log(response);
        this.toastr.success('New branch created!');
      },
      badRequest => {
        console.log(badRequest);
        if (Array.isArray(badRequest.error.BranchCode))
          badRequest.error.BranchCode.forEach(element => {
            if (element == 'Branch code is already in use.')
              this.toastr.error('Branch code is already in use.', 'Branch creation failed.');
          });
        else
          this.toastr.error('Branch creation failed');
      }
    );
  }
}
