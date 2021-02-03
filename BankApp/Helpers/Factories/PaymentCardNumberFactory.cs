using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Helpers.Builders;
using BankApp.Interfaces;
using BankApp.Models;

namespace BankApp.Helpers.Factories
{
    public class PaymentCardNumberFactory : IPaymentCardNumberFactory
    {
        private readonly BankIdentificationNumberData _bankIdentificationNumberData;

        public PaymentCardNumberFactory(BankIdentificationNumberData bankIdentificationNumberData)
        {
            _bankIdentificationNumberData = bankIdentificationNumberData;
        }

        public IPaymentCardNumberBuilder GetPaymentCardNumberBuilder(IssuingNetwork issuingNetwork)
        {
            if (issuingNetwork == IssuingNetwork.Mastercard)
                return new MastercardPaymentCardNumberBuilder(_bankIdentificationNumberData);

            return new VisaPaymentCardNumberBuilder(_bankIdentificationNumberData);
        }
    }
}
