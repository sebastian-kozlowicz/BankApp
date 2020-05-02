import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup, AbstractControl } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from "../../../services/auth.service";
import { PasswordValidator } from "../../../validators/password-validator";
import { Register } from "../../../models/register";

@Component({
    selector: 'app-registration',
    templateUrl: './registration.component.html',
    styleUrls: ['./registration.component.css']
})
export class CustomerRegistrationComponent implements OnInit {

    constructor(private fb: FormBuilder, private authService: AuthService, private toastr: ToastrService) { }

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

    get name() {
        return this.registerFormModel.get('name');
    }
    get surname() {
        return this.registerFormModel.get('surname');
    }
    get email() {
        return this.registerFormModel.get('email');
    }
    get password() {
        return this.registerFormModel.get('password');
    }
    get confirmPassword() {
        return this.registerFormModel.get('confirmPassword');
    }

    registerFormModel = this.fb.group({
        name: ['', Validators.required],
        surname: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
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

    isErrorOccur(fg: AbstractControl, error) {
        return fg.errors?.[error];
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

    onSubmit() {
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
