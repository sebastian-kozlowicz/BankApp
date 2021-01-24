using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System.Linq;

namespace BankApp.Helpers.Builders
{
    public class MastercardPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        //private static readonly int MastercardPrefix = 4;
        private readonly ApplicationDbContext _context;

        public MastercardPaymentCardNumberBuilder(ApplicationDbContext context)
        {
            var bankIdentificationNumber = GetBankIdentificationNumber();
            _context = context;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length)
        {
            return null;
        }

        private BankIdentificationNumberData GetBankIdentificationNumber()
        {
            return _context.BankIdentificationNumberData.FirstOrDefault(bin => bin.IssuingNetwork == IssuingNetwork.Mastercard);
        }
    }
}
