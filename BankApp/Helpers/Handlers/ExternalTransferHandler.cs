using System.Threading.Tasks;
using BankApp.Interfaces.Helpers.Handlers;
using BankApp.Models;

namespace BankApp.Helpers.Handlers
{
    public class ExternalTransferHandler : ITransferHandler<ExternalTransferHandler>
    {
        /// <summary>
        ///     Method that fakes real transfer order in external system like Elixir
        /// </summary>
        public async Task CreateBankTransferAsync(BankAccount bankAccount, BankAccount targetBankAccount, decimal value)
        {
        }
    }
}