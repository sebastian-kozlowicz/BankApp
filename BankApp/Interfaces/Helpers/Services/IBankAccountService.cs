using System.Collections.Generic;
using System.Threading.Tasks;
using BankApp.Dtos.BankAccount.WithCustomerCreation;
using BankApp.Models;
using BankAccountCreationDto = BankApp.Dtos.BankAccount.BankAccountCreationDto;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface IBankAccountService
    {
        Task<BankAccount> GetBankAccountAsync(int bankAccountId);
        Task<IEnumerable<BankAccount>> GetBankAccountsForUserAsync(int applicationUserId);
        Task<BankAccount> CreateBankAccountAsync(BankAccountCreationDto model);

        Task<BankAccount> CreateBankAccountWithCustomerByCustomerAsync(
            BankAccountWithCustomerCreationByCustomerDto model);

        Task<BankAccount> CreateBankAccountWithCustomerByWorkerAsync(
            BankAccountWithCustomerCreationByWorkerDto model, int currentUserId);
    }
}