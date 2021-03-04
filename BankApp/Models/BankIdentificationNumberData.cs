using BankApp.Enumerators;

namespace BankApp.Models
{
    public class BankIdentificationNumberData
    {
        public int Id { get; set; }
        public int BankIdentificationNumber { get; set; }
        public IssuingNetwork IssuingNetwork { get; set; }
    }
}
