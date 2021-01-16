using BankApp.Enumerators;
using BankApp.Helpers.Builders;
using BankApp.Interfaces;

namespace BankApp.Helpers.Factories
{
    public class PaymentCardNumberFactory : IPaymentCardNumberFactory
    {
        public IPaymentCardNumberBuilder GetPaymentCardNumberBuilder(IssuingNetwork issuingNetwork)
        {
            if (issuingNetwork == IssuingNetwork.Mastercard)
                return new MastercardPaymentCardNumberBuilder();

            return new VisaPaymentCardNumberBuilder();
        }
    }
}
