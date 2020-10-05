import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-new-branch',
  templateUrl: './new-branch.component.html',
  styleUrls: ['./new-branch.component.css']
})
export class NewBranchComponent implements OnInit {

  constructor(private fb: FormBuilder) { }

  get branchCode() {
    return this.branchForm.get('branchCode');
  }

  ngOnInit(): void {
  }

  branchForm = this.fb.group({
    branchCode: ['', Validators.required],
    address: [null, Validators.required]
  });
}
