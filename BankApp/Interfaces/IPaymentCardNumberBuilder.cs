namespace BankApp.Interfaces
{
    public interface IPaymentCardNumberBuilder
    {
        string GenerateCardNumber(int length);
    }
}
