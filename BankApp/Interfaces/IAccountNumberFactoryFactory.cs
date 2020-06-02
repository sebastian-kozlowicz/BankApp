using BankApp.Models;

namespace BankApp.Interfaces
{
    interface IAccountNumberFactory
    {
        BankAccount GenerateAccountNumber(string branchId);
    }
}
