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
        private readonly BankAccount _bankAccount;

        public PaymentCardNumberFactory(BankIdentificationNumberData bankIdentificationNumberData, BankAccount bankAccount)
        {
            _bankIdentificationNumberData = bankIdentificationNumberData;
            _bankAccount = bankAccount;
        }

        public IPaymentCardNumberBuilder GetPaymentCardNumberBuilder(IssuingNetwork issuingNetwork)
        {
            if (issuingNetwork == IssuingNetwork.Mastercard)
                return new MastercardPaymentCardNumberBuilder(_bankIdentificationNumberData, _bankAccount);

            return new VisaPaymentCardNumberBuilder(_bankIdentificationNumberData, _bankAccount);
        }
    }
}
