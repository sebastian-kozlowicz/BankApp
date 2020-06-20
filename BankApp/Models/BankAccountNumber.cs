namespace BankApp.Models
{
    public class BankAccountNumber
    {
        public string CountryCode { get; set; }
        public string CheckNumber { get; set; }
        public string NationalBankCode { get; set; }
        public string BranchCode { get; set; }
        public int NationalCheckDigit { get; set; }
        public long AccountNumber { get; set; }
        public string AccountNumberText { get; set; }
        public string Iban { get; set; }
        public string IbanSeparated { get; set; }
    }
}
