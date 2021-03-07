using BankApp.Enumerators;

namespace BankApp.Dtos.Card
{
    public class PaymentCardDto
    {
        public int Id { get; set; }
        public byte MajorIndustryIdentifier { get; set; }
        public int BankIdentificationNumber { get; set; }
        public long AccountIdentificationNumber { get; set; }
        public string AccountIdentificationNumberText { get; set; }
        public byte CheckDigit { get; set; }
        public string Number { get; set; }
        public IssuingNetwork IssuingNetwork { get; set; }
        public int BankAccountId { get; set; }
    }
}
