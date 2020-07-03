using BankApp.Models;

namespace BankApp.Interfaces
{
    public interface IAccountNumberFactory
    {
        BankAccountNumber GenerateAccountNumber(int? branchId = null);
    }
}
