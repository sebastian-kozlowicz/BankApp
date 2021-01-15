namespace BankApp.Interfaces
{
    public interface IPaymentCardNumberGenerator<T> where T : class, IPaymentCardNumberGenerator<T>
    {
        string GenerateCardNumber(int length);
    }
}
