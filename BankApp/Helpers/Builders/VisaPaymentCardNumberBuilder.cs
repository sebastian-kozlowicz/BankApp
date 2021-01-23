using BankApp.Data;
using BankApp.Interfaces;
using BankApp.Models;

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
            return null;
        }
    }
}
