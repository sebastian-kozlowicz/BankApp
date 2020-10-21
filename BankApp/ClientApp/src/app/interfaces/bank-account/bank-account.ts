import { AccountType } from "../../enumerators/accountType";
import { Currency } from "../../enumerators/currency";

export interface BankAccount {
  id: number;
  accountType: AccountType;
  currency: Currency;
  countryCode: string;
  checkNumber: string;
  nationalBankCode: string;
  branchCode: string;
  nationalCheckDigit: number;
  accountNumber: number;
  accountNumberText: string;
  iban: string;
  ibanSeparated: string;
  balance: number;
  debitLimit: number;
  customerId: number;
}
