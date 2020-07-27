using BankApp.Data;
using BankApp.Models;

namespace BankApp.Helpers
{
    public class InternalTransferService
    {
        private readonly ApplicationDbContext _context;

        public InternalTransferService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(BankAccount bankAccount, BankAccount targetBankAccount, decimal value)
        {
            if (bankAccount.Currency != targetBankAccount.Currency)
                return;

            bankAccount.Balance -= value;
            targetBankAccount.Balance += value;

            _context.SaveChanges();
        }
    }
}
