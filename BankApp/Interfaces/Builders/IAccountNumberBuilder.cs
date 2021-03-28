using BankApp.Models;

namespace BankApp.Interfaces.Builders
{
    public interface IBankAccountNumberBuilder
    {
        BankAccountNumber GenerateBankAccountNumber(int? branchId = null);
    }
}
