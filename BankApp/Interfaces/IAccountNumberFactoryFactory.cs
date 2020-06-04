using BankApp.Models;

namespace BankApp.Interfaces
{
    interface IAccountNumberFactory
    {
        BankAccountNumber GenerateAccountNumber(string branchId);
    }
}
