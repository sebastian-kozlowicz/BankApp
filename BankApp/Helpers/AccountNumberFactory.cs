﻿using BankApp.Data;
using BankApp.Interfaces;
using BankApp.Models;
using System.Collections.Generic;
using System.Linq;
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

        public virtual BankAccountNumber GenerateAccountNumber(string branchId = null)
        {
            var bankData = GetBankData();
            var branchCode = GetBranchCode(branchId);
            var nationalCheckDigit = GenerateNationalCheckDigit(bankData.NationalBankCode, branchCode);
            var accountNumber = GenerateAccountNumber();
            var accountNumberText = GetAccountNumberText(accountNumber);
            var checkNumber = GenerateCheckNumber(bankData, branchCode, nationalCheckDigit, accountNumberText);
            var iban = GetIban(bankData, checkNumber, branchCode, nationalCheckDigit, accountNumberText);
            var ibanSeparated = GetIbanSeparated(bankData, checkNumber, branchCode, nationalCheckDigit, accountNumberText);

            return new BankAccountNumber
            {
                CountryCode = bankData.CountryCode,
                CheckNumber = checkNumber,
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
            var nationalBankCodeArray = nationalBankCode.Select(digit => int.Parse(digit.ToString())).ToArray();
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

            var splittedAccountNumberArray = Regex.Split(formatedAccountNumber, "(?<=\\G.{8})");

            string modResult = string.Empty;
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
            var splittedAccountNumberArray = Regex.Split(accountNumberText, "(?<=\\G.{4})");
            splittedAccountNumberArray = splittedAccountNumberArray.Where(an => !string.IsNullOrEmpty(an)).ToArray();
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

        private string GetBranchCode(string branchId)
        {
            if (branchId == null)
                return _context.Branches.SingleOrDefault(b => b.Id == _context.Headquarters.SingleOrDefault().Id).BranchCode;

            return _context.Branches.SingleOrDefault(b => b.Id == branchId).BranchCode;
        }

        private long GenerateAccountNumber()
        {
            if (_context.BankAccounts.Select(ba => ba.AccountNumber).ToList() is var result && !result.Any())
                return 0;

            return result.Max() + 1;
        }

        private string GetAccountNumberText(long accountNumber)
        {
            return accountNumber.ToString("D16");
        }
    }
}