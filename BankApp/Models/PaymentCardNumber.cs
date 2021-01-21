using BankApp.Enumerators;

namespace BankApp.Models
{
    public class PaymentCardNumber
    {
        public byte MajorIndustryIdentifier { get; set; }
        public int BankIdentificationNumber { get; set; }
        public long AccountIdentificationNumber { get; set; }
        public byte CheckDigit { get; set; }
        public string Number { get; set; }
        public IssuingNetwork IssuingNetwork { get; set; }
    }
}
