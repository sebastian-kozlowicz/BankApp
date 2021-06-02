using BankApp.Enumerators;
using BankApp.Interfaces.Builders;
using BankApp.Interfaces.Builders.Number;

namespace BankApp.Interfaces.Factories
{
    public interface IPaymentCardNumberFactory
    {
        IPaymentCardNumberBuilder GetPaymentCardNumberBuilder(IssuingNetwork issuingNetwork);
    }
}