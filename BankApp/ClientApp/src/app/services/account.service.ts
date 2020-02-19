import { Injectable } from '@angular/core';
import { FormBuilder } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private fb: FormBuilder) { }

  registerModel = this.fb.group({
    name: [''],
    surname: [''],
    email: [''],
    password: [''],
    confirmPassword: ['']
  });
}
