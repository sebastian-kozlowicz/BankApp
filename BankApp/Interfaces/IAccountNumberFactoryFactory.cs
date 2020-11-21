using BankApp.Models;

namespace BankApp.Interfaces
{
    public interface IAccountNumberFactory
    {
        BankAccountNumber GenerateBankAccountNumber(int? branchId = null);
    }
}
