import { Register } from "../../auth/register"
import { BankAccountCreation } from "./bank-account-creation"

export interface BankAccountWithCustomerCreation {
  register: Register;
  bankAccount: BankAccountCreation;
}
