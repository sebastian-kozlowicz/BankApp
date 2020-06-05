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
            var nationalCheckDigit = GenerateNationalCheckDigit(bankData.NationalBankCode, branchCode);
            var accountNumber = GenerateAccountNumber();
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

        private int GenerateNationalCheckDigit(int nationalBankCode, int branchCode)
        {
            int sum = 0;
            var weights = new int[] { 3, 9, 7, 1, 3, 9, 7, 1 };
            var nationalBankCodeArray = nationalBankCode.ToString().Select(digit => int.Parse(digit.ToString())).ToArray();
            var branchCodeArray = branchCode.ToString().Select(digit => int.Parse(digit.ToString())).ToArray();

            for (int i = 0; i < weights.Length; i++)
            {
                if (i < nationalBankCodeArray.Length)
                    sum += weights[i] * nationalBankCodeArray[i];
                else
                    sum += weights[i] * branchCodeArray[i];
            }

            return sum;
        }

        private long GenerateAccountNumber()
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
