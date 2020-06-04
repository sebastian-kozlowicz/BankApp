using BankApp.Data;
using BankApp.Interfaces;
using BankApp.Models;
using System.Linq;

namespace BankApp.Helpers
{
    public class AccountNumberFactory : IAccountNumberFactory
    {
        private readonly ApplicationDbContext _context;

        public AccountNumberFactory(ApplicationDbContext context)
        {
            _context = context;
        }

        public BankAccountNumber GenerateAccountNumber(string branchId)
        {
            var bankData = GetBankData();
            var branchCode = GetBranchCode(branchId);
            var accountNumber = GetAccountNumber();
            var accountNumberText = GetAccountNumberText(accountNumber);

            return new BankAccountNumber();
        }

        private BankData GetBankData()
        {
            return _context.BankData.LastOrDefault();
        }

        private int GetBranchCode(string branchId)
        {
            if (branchId == null)
                return _context.Branches.SingleOrDefault(b => b.Id == _context.Headquarters.SingleOrDefault().Id).BranchCode;

            return _context.Branches.SingleOrDefault(b => b.Id == branchId).BranchCode;
        }

        private long GetAccountNumber()
        {
            if (_context.BankAccounts.Select(ba => ba.AccountNumber).DefaultIfEmpty(0).Max() is var accountNumber && accountNumber == 0)
                return accountNumber;

            return accountNumber++;
        }

        private string GetAccountNumberText(long accountNumber)
        {
            return accountNumber.ToString("D16");
        }
    }
}
