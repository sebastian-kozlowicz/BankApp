namespace BankApp.Models
{
    public class BankAccountNumber
    {
        public string CountryCode { get; set; }
        public int CheckNumber { get; set; }
        public int NationalBankCode { get; set; }
        public int BranchCode { get; set; }
        public int NationalCheckDigit { get; set; }
        public long AccountNumber { get; set; }
        public string AccountNumberText { get; set; }
        public string Iban { get; set; }
    }
}
