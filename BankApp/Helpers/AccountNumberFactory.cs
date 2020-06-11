using BankApp.Data;
using BankApp.Interfaces;
using BankApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BankApp.Helpers
{
    public class AccountNumberFactory : IAccountNumberFactory
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<string, int> _countryCharactersAssignedToNumbers = new Dictionary<string, int>()
        {
            {"A", 10 },
            {"B", 11 },
            {"C", 12 },
            {"D", 13 },
            {"E", 14 },
            {"F", 15 },
            {"G", 16 },
            {"H", 17 },
            {"I", 18 },
            {"J", 19 },
            {"K", 20 },
            {"L", 21 },
            {"M", 22 },
            {"N", 23 },
            {"O", 24 },
            {"P", 25 },
            {"Q", 26 },
            {"R", 27 },
            {"S", 28 },
            {"T", 29 },
            {"U", 30 },
            {"V", 31 },
            {"W", 32 },
            {"X", 33 },
            {"Y", 34 },
            {"Z", 35 },
        };

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
            var checkNumber = GenerateCheckNumber(bankData, branchCode, nationalCheckDigit, accountNumberText);

            return new BankAccountNumber();
        }

        private BankData GetBankData()
        {
            return _context.BankData.LastOrDefault();
        }

        private string GetBranchCode(string branchId)
        {
            if (branchId == null)
                return _context.Branches.SingleOrDefault(b => b.Id == _context.Headquarters.SingleOrDefault().Id).BranchCode;

            return _context.Branches.SingleOrDefault(b => b.Id == branchId).BranchCode;
        }

        public int GenerateNationalCheckDigit(int nationalBankCode, string branchCode)
        {
            int sum = 0;
            var weights = new int[] { 3, 9, 7, 1, 3, 9, 7 };
            var nationalBankCodeArray = nationalBankCode.ToString().Select(digit => int.Parse(digit.ToString())).ToArray();
            var branchCodeArray = branchCode.Select(digit => int.Parse(digit.ToString())).ToArray();

            for (int i = 0; i < nationalBankCodeArray.Length; i++)
                sum += weights[i] * nationalBankCodeArray[i];

            int j = 0;
            for (int i = nationalBankCodeArray.Length; i < weights.Length; i++)
            {
                sum += weights[i] * branchCodeArray[j];
                j++;
            }

            return (10 - sum % 10) % 10;
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

        public string GenerateCheckNumber(BankData bankData, string branchCode, int nationalCheckDigit, string accountNumberText)
        {
            var firstCountryCharacterAsNumber = _countryCharactersAssignedToNumbers[bankData.CountryCode.Substring(0, 1)];
            var secondCountryCharacterAsNumber = _countryCharactersAssignedToNumbers[bankData.CountryCode.Substring(1, 1)];

            var formatedAccountNumber = $"{bankData.NationalBankCode}" +
                                        $"{branchCode}" +
                                        $"{nationalCheckDigit}" +
                                        $"{accountNumberText}" +
                                        $"{firstCountryCharacterAsNumber}" +
                                        $"{secondCountryCharacterAsNumber}"
                                        + "00";

            var splittedAccountNumber = Regex.Split(formatedAccountNumber, "(?<=\\G.{8})");

            string modResult = string.Empty;
            foreach (var number in splittedAccountNumber)
            {
                modResult = (long.Parse(modResult + number) % 97).ToString();
            }

            return (98 - int.Parse(modResult)).ToString();
        }
    }
}
