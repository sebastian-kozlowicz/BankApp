using BankApp.Enumerators;
using BankApp.Interfaces.Helpers.Builders.Number;

namespace BankApp.Interfaces.Helpers.Factories
{
    public interface IPaymentCardNumberFactory
    {
        IPaymentCardNumberBuilder GetPaymentCardNumberBuilder(IssuingNetwork issuingNetwork);
    }
}