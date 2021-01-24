using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System.Linq;

namespace BankApp.Helpers.Builders
{
    public class VisaPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private static readonly int VisaPrefix = 4;
        private readonly ApplicationDbContext _context;

        public VisaPaymentCardNumberBuilder(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length)
        {
            var bankIdentificationNumber = GetBankIdentificationNumber();
            return null;
        }

        private BankIdentificationNumberData GetBankIdentificationNumber()
        {
            return _context.BankIdentificationNumberData.FirstOrDefault(bin => bin.IssuingNetwork == IssuingNetwork.Visa);
        }
    }
}
