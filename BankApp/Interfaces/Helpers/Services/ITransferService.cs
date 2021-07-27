using BankApp.Models;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface ITransferService<T> where T : class, ITransferService<T>
    {
        void Create(BankAccount bankAccount, BankAccount targetBankAccount, decimal value);
    }
}
