import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export class NumberLimitValidator {
  static limitValidator(min: number, max: number, error: ValidationErrors): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      if (control.value && (isNaN(control.value) || control.value < min || control.value > max))
        return error;

      return null;
    };
  }
}
