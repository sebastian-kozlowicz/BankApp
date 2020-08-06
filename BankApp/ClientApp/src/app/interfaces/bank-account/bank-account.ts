import { AccountType } from "../../enumerators/accountType";
import { Currency } from "../../enumerators/currency";

export interface BankAccount {
  id: number;
  accountType: AccountType;
  currency: Currency;
  countryCode: string;
  checkNumber: string;
  branchCode: string;
  nationalCheckDigit: number;
  iban: string;
  ibanSeparated: string;
  balance: number;
  debitLimit: number;
  applicationUserId: number;
}
