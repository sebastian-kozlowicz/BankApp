using BankApp.Data;
using BankApp.Interfaces;
using BankApp.Models;

namespace BankApp.Helpers.Builders
{
    public class MastercardPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        //private static readonly int MastercardPrefix = 4;
        private readonly ApplicationDbContext _context;

        public MastercardPaymentCardNumberBuilder(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length)
        {
            return null;
        }
    }
}
