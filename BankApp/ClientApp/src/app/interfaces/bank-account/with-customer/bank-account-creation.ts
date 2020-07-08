import { AccountType } from "../../../enumerators/accountType";
import { Currency } from "../../../enumerators/currency";

export interface BankAccountCreation {
  accountType: AccountType;
  currency: Currency;
}
