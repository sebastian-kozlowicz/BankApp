import { AbstractControl, ValidationErrors, ValidatorFn, FormGroup } from "@angular/forms";

export class PasswordValidator {
  static patternValidator(regex: RegExp, error: ValidationErrors): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      if (!control.value) {
        // if control is empty return no error
        return null;
      }

      // test the value of the control against the regexp supplied
      const valid = regex.test(control.value);

      // if true, return no error (no error), else return error passed in the second parameter
      return valid ? null : error;
    };
  }

  static isPasswordMatch(fg: FormGroup) {
    const password = fg.controls['password'];
    const confirmPassword = fg.controls['confirmPassword'];
    const errors = { ...confirmPassword.errors };
    delete errors.passwordMismatch;

    if (confirmPassword.value && password.value !== confirmPassword.value) {
      errors.passwordMismatch = true;
    }

    confirmPassword.setErrors(Object.keys(errors).length ? errors : null);
    return null;
  }

  static isErrorOccur(ac: AbstractControl, error) {
    return ac.errors?.[error];
  }
}
