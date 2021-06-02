using BankApp.Models;

namespace BankApp.Interfaces.Builders.Number
{
    public interface IPaymentCardNumberBuilder
    {
        PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId);
        byte GenerateCheckDigit(string number);
    }
}
