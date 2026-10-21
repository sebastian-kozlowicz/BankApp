import { AccountType } from '../../enumerators/accountType';
import { Currency } from '../../enumerators/currency';

export interface CurrentCustomerBankAccountCreation {
  accountType: AccountType;
  currency: Currency;
}
