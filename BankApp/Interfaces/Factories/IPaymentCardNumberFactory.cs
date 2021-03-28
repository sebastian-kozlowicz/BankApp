using BankApp.Enumerators;
using BankApp.Interfaces.Builders;

namespace BankApp.Interfaces.Factories
{
    public interface IPaymentCardNumberFactory
    {
        IPaymentCardNumberBuilder GetPaymentCardNumberBuilder(IssuingNetwork issuingNetwork);
    }
}