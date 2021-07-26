using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Helpers.Builders.Number;
using BankApp.Interfaces.Helpers.Builders.Number;
using BankApp.Interfaces.Helpers.Factories;

namespace BankApp.Helpers.Factories
{
    public class PaymentCardNumberFactory : IPaymentCardNumberFactory
    {
        private readonly ApplicationDbContext _context;

        public PaymentCardNumberFactory(ApplicationDbContext context)
        {
            _context = context;
        }

        public IPaymentCardNumberBuilder GetPaymentCardNumberBuilder(IssuingNetwork issuingNetwork)
        {
            if (issuingNetwork == IssuingNetwork.Mastercard)
                return new MastercardPaymentCardNumberBuilder(_context);

            return new VisaPaymentCardNumberBuilder(_context);
        }
    }
}