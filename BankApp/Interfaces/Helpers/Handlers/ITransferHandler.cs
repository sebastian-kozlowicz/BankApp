using System.Threading.Tasks;
using BankApp.Models;

namespace BankApp.Interfaces.Helpers.Handlers
{
    public interface ITransferHandler<T> where T : class, ITransferHandler<T>
    {
        Task CreateBankTransferAsync(BankAccount bankAccount, BankAccount targetBankAccount, decimal value);
    }
}
