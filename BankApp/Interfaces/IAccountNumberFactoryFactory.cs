using BankApp.Models;

namespace BankApp.Interfaces
{
    interface IAccountNumberFactory
    {
        BankAccountNumber GenerateAccountNumber(int? branchId);
    }
}
