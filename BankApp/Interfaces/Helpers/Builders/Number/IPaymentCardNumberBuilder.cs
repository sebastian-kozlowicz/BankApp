using BankApp.Models;

namespace BankApp.Interfaces.Helpers.Builders.Number
{
    public interface IPaymentCardNumberBuilder
    {
        PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId);
        byte GenerateCheckDigit(string number);
        bool ValidatePaymentCardNumber(string paymentCardNumber);
    }
}
