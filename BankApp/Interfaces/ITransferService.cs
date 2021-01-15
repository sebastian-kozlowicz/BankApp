using BankApp.Models;

namespace BankApp.Interfaces
{
    public interface ITransferService<T> where T : class, ITransferService<T>
    {
        void Create(BankAccount bankAccount, BankAccount targetBankAccount, decimal value);
    }
}
