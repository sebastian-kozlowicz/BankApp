using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BankApp.Data;
using BankApp.Interfaces;
using BankApp.Interfaces.Builders;
using BankApp.Models;

namespace BankApp.Helpers.Builders
{
    public class BankAccountNumberBuilder : IBankAccountNumberBuilder
    {
        private readonly ApplicationDbContext _context;
        private static readonly Dictionary<string, int> CountryCharactersAssignedToNumbers = new Dictionary<string, int>
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

        public BankAccountNumberBuilder(ApplicationDbContext context)
        {
            _context = context;
        }

        public BankAccountNumber GenerateBankAccountNumber(int? branchId = null)
        {
            var bankData = GetBankData();
            var branchCode = GetBranchCode(branchId);
            var nationalCheckDigit = GenerateNationalCheckDigit(bankData.NationalBankCode, branchCode);
            var accountNumber = GenerateAccountNumber();
            var accountNumberText = GetAccountNumberText(accountNumber);
            var checkDigits = GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);
            var iban = GetIban(bankData, checkDigits, branchCode, nationalCheckDigit, accountNumberText);
            var ibanSeparated = GetIbanSeparated(bankData, checkDigits, branchCode, nationalCheckDigit, accountNumberText);

            return new BankAccountNumber
            {
                CountryCode = bankData.CountryCode,
                CheckDigits = checkDigits,
                NationalBankCode = bankData.NationalBankCode,
                BranchCode = branchCode,
                NationalCheckDigit = nationalCheckDigit,
                AccountNumber = accountNumber,
                AccountNumberText = accountNumberText,
                Iban = iban,
                IbanSeparated = ibanSeparated
            };
        }

        public int GenerateNationalCheckDigit(string nationalBankCode, string branchCode)
        {
            int sum = 0;
            var weights = new int[] { 3, 9, 7, 1, 3, 9, 7 };
            var nationalBankCodeDigitsArray = nationalBankCode.Select(digit => int.Parse(digit.ToString())).ToArray();
            var branchCodeArray = branchCode.Select(digit => int.Parse(digit.ToString())).ToArray();
            var concatenatedDigitsArray = nationalBankCodeDigitsArray.Concat(branchCodeArray).ToArray();

            for (int i = 0; i < concatenatedDigitsArray.Length; i++)
                sum += weights[i] * concatenatedDigitsArray[i];

            return (10 - sum % 10) % 10;
        }

        public string GenerateCheckDigits(BankData bankData, string branchCode, int nationalCheckDigit, string accountNumberText)
        {
            var firstCountryCharacterAsNumber = CountryCharactersAssignedToNumbers[bankData.CountryCode.Substring(0, 1)];
            var secondCountryCharacterAsNumber = CountryCharactersAssignedToNumbers[bankData.CountryCode.Substring(1, 1)];

            var formattedAccountNumber = $"{bankData.NationalBankCode}" +
                                         $"{branchCode}" +
                                         $"{nationalCheckDigit}" +
                                         $"{accountNumberText}" +
                                         $"{firstCountryCharacterAsNumber}" +
                                         $"{secondCountryCharacterAsNumber}"
                                         + "00";

            var splittedAccountNumberArray = Regex.Split(formattedAccountNumber, "(?<=\\G.{8})");

            var modResult = string.Empty;
            foreach (var number in splittedAccountNumberArray)
            {
                modResult = (long.Parse(modResult + number) % 97).ToString();
            }

            var checkNumber = (98 - int.Parse(modResult)).ToString();

            if (checkNumber.Length > 1)
                return checkNumber;

            return "0" + checkNumber;
        }

        public string GetIban(BankData bankData, string checkNumber, string branchCode, int nationalCheckDigit, string accountNumberText)
        {
            return $"{bankData.CountryCode}" +
                   $"{checkNumber}" +
                   $"{bankData.NationalBankCode}" +
                   $"{branchCode}" +
                   $"{nationalCheckDigit}" +
                   $"{accountNumberText}";
        }

        public string GetIbanSeparated(BankData bankData, string checkNumber, string branchCode, int nationalCheckDigit, string accountNumberText)
        {
            var splittedAccountNumberArray = Regex.Split(accountNumberText, "(?<=\\G.{4})").Where(an => !string.IsNullOrEmpty(an)).ToArray();
            var separatedAccountNumber = string.Join(" ", splittedAccountNumberArray);

            return $"{bankData.CountryCode} " +
                   $"{checkNumber} " +
                   $"{bankData.NationalBankCode} " +
                   $"{branchCode}" +
                   $"{nationalCheckDigit} " +
                   $"{separatedAccountNumber}";
        }

        private BankData GetBankData()
        {
            return _context.BankData.FirstOrDefault();
        }

        private string GetBranchCode(int? branchId)
        {
            if (branchId == null)
            {
                var headquarters = _context.Branches.SingleOrDefault(b => b.Id == _context.Headquarters.FirstOrDefault().Id);
                if (headquarters == null)
                    throw new Exception("Headquarters doesn't exist in database.");

                return headquarters.BranchCode;
            }

            var branch = _context.Branches.SingleOrDefault(b => b.Id == branchId);
            if (branch == null)
                throw new ArgumentException($"Branch with id {branchId} doesn't exist.");

            return branch.BranchCode;
        }

        private long GenerateAccountNumber()
        {
            var maxAccountNumber = _context.BankAccounts.Max(b => (long?)b.AccountNumber) ?? -1;

            if (maxAccountNumber == -1)
                return 0;

            return maxAccountNumber + 1;
        }

        private string GetAccountNumberText(long accountNumber)
        {
            return accountNumber.ToString("D16");
        }
    }
}