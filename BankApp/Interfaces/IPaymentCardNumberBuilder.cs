using BankApp.Models;

namespace BankApp.Interfaces
{
    public interface IPaymentCardNumberBuilder
    {
        PaymentCardNumber GeneratePaymentCardNumber(int length);
    }
}
