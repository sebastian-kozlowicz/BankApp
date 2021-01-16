using BankApp.Models;

namespace BankApp.Interfaces
{
    public interface IBankAccountNumberBuilder
    {
        BankAccountNumber GenerateBankAccountNumber(int? branchId = null);
    }
}
