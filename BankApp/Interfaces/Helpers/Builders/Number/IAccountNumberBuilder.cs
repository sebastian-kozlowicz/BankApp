using BankApp.Models;

namespace BankApp.Interfaces.Helpers.Builders.Number
{
    public interface IBankAccountNumberBuilder
    {
        BankAccountNumber GenerateBankAccountNumber(int? branchId = null);
    }
}
