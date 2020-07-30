export interface BankTransferCreation {
  requesterBankAccountId: number;
  receiverIban: string;
  value: number;
}
