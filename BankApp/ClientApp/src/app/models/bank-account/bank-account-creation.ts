import { AccountType } from "../../enumerators/accountType";
import { Currency } from "../../enumerators/currency";

export class BankAccountCreation {
  accountType: AccountType;
  currency: Currency;
  applicationUserId: string;
}
