using BankApp.Models;

namespace BankApp.Interfaces.Builders.Number
{
    public interface IBankAccountNumberBuilder
    {
        BankAccountNumber GenerateBankAccountNumber(int? branchId = null);
    }
}
