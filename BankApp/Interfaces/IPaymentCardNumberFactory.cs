using BankApp.Enumerators;

namespace BankApp.Interfaces
{
    public interface IPaymentCardNumberFactory
    {
        IPaymentCardNumberBuilder GetPaymentCardNumberBuilder(IssuingNetwork issuingNetwork);
    }
}