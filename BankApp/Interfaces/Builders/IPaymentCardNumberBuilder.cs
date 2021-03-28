using BankApp.Models;

namespace BankApp.Interfaces.Builders
{
    public interface IPaymentCardNumberBuilder
    {
        PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId);
    }
}
