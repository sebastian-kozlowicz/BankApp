using BankApp.Models;

namespace BankApp.Interfaces
{
    public interface IBankAccountNumberFactory
    {
        BankAccountNumber GenerateBankAccountNumber(int? branchId = null);
    }
}
