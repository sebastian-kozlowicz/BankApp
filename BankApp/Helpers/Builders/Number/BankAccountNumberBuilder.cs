using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Exceptions;
using BankApp.Interfaces.Helpers.Builders.Number;
using BankApp.Models;

namespace BankApp.Helpers.Builders.Number
{
    /// <summary>
    ///     Polish IBAN builder
    ///     Polish IBAN structure: PLkk bbbb sssx cccc cccc cccc cccc
    ///     where:
    ///     k = Check digits
    ///     b = National bank code - 4 digit number assigned to bank
    ///     s = Branch code
    ///     x = National check digit
    ///     c = Account number
    ///     Detailed explanation of IBAN structure, generation and validation is available on Wikipedia
    ///     https://en.wikipedia.org/wiki/International_Bank_Account_Number
    ///     Important note - National bank code of polish IBAN is 4 digit number, there is false information saying it is 3
    ///     digits on Wikipedia
    ///     Polish website explaining polish IBAN structure with example of bank identification by National bank code 4 digit
    ///     number https://www.bankier.pl/smart/numer-konta-bankowego-gdzie-jest-ile-ma-cyfr-przyklad
    /// </summary>
    public class BankAccountNumberBuilder : IBankAccountNumberBuilder
    {
        private static readonly Dictionary<string, int> CountryCodeCharactersAssignedToNumbers = new()
        {
            { "A", 10 },
            { "B", 11 },
            { "C", 12 },
            { "D", 13 },
            { "E", 14 },
            { "F", 15 },
            { "G", 16 },
            { "H", 17 },
            { "I", 18 },
            { "J", 19 },
            { "K", 20 },
            { "L", 21 },
            { "M", 22 },
            { "N", 23 },
            { "O", 24 },
            { "P", 25 },
            { "Q", 26 },
            { "R", 27 },
            { "S", 28 },
            { "T", 29 },
            { "U", 30 },
            { "V", 31 },
            { "W", 32 },
            { "X", 33 },
            { "Y", 34 },
            { "Z", 35 }
        };

        private readonly ApplicationDbContext _context;

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
            var ibanSeparated =
                GetIbanSeparated(bankData, checkDigits, branchCode, nationalCheckDigit, accountNumberText);

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

        /// <summary>
        ///     Weighted sum: Treats the account number as a series of individual numbers, multiplies each number by a weight value
        ///     according to its position in the string, sums the products, divides the sum by a modulus 10 and subtracts 10 from
        ///     the reminder
        /// </summary>
        /// <param name="nationalBankCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        public int GenerateNationalCheckDigit(string nationalBankCode, string branchCode)
        {
            ThrowIfStringIsNotNumber(nationalBankCode, nameof(nationalBankCode));
            ThrowIfStringIsNotNumber(branchCode, nameof(branchCode));

            if(nationalBankCode.Length != NumberLengthSettings.BankAccount.NationalBankCode)
                throw new ArgumentException(
                    $"National bank code should be length of {NumberLengthSettings.BankAccount.NationalBankCode} numbers.");
            
            if(branchCode.Length != NumberLengthSettings.BankAccount.BranchCode)
                throw new ArgumentException(
                    $"Branch code should be length of {NumberLengthSettings.BankAccount.BranchCode} numbers.");

            var weights = new[] { 3, 9, 7, 1, 3, 9, 7 };
            var nationalBankCodeDigitsArray = nationalBankCode.Select(digit => int.Parse(digit.ToString())).ToArray();
            var branchCodeDigitsArray = branchCode.Select(digit => int.Parse(digit.ToString())).ToArray();
            var concatenatedDigitsArray = nationalBankCodeDigitsArray.Concat(branchCodeDigitsArray).ToArray();

            var sum = concatenatedDigitsArray.Select((digit, i) => weights[i] * digit).Sum();

            return 10 - sum % 10;
        }

        /// <summary>
        ///     Checks that the total IBAN length is correct as per the country. If not, the IBAN is invalid.
        ///     Replaces the two check digits by 00 (e.g., GB00 for the UK).
        ///     Moves the four initial characters to the end of the string.
        ///     Replaces the letters in the string with digits, expanding the string as necessary, such that A or a = 10, B or b =
        ///     11, and Z or z = 35. Each alphabetic character is therefore replaced by 2 digits
        ///     Converts the string to an long (i.e.ignore leading zeroes).
        ///     Calculates mod-97 of the new number, which results in the remainder.
        ///     Subtracts the remainder from 98 and use the result for the two check digits. If the result is a single-digit
        ///     number,
        ///     pads it with a leading 0 to make a two-digit number.
        /// </summary>
        /// <param name="bankData"></param>
        /// <param name="branchCode"></param>
        /// <param name="nationalCheckDigit"></param>
        /// <param name="accountNumberText"></param>
        /// <returns></returns>
        public string GenerateCheckDigits(BankData bankData, string branchCode, int nationalCheckDigit,
            string accountNumberText)
        {
            ThrowIfStringIsNotNumber(branchCode, nameof(branchCode));
            ThrowIfStringIsNotNumber(accountNumberText, nameof(accountNumberText));

            if (branchCode.Length != NumberLengthSettings.BankAccount.BranchCode)
                throw new ArgumentException(
                    $"Branch code should be length of {NumberLengthSettings.BankAccount.BranchCode} numbers.");  
            
            if (branchCode.Length != NumberLengthSettings.BankAccount.BranchCode)
                throw new ArgumentException(
                    $"Branch code should be length of {NumberLengthSettings.BankAccount.BranchCode} numbers.");

            var firstCountryCodeCharacterAsNumber =
                CountryCodeCharactersAssignedToNumbers[bankData.CountryCode.Substring(0, 1)];
            var secondCountryCharacterAsNumber =
                CountryCodeCharactersAssignedToNumbers[bankData.CountryCode.Substring(1, 1)];

            var formattedAccountNumber = $"{bankData.NationalBankCode}" +
                                         $"{branchCode}" +
                                         $"{nationalCheckDigit}" +
                                         $"{accountNumberText}" +
                                         $"{firstCountryCodeCharacterAsNumber}" +
                                         $"{secondCountryCharacterAsNumber}"
                                         + "00";

            if (formattedAccountNumber.Length != NumberLengthSettings.BankAccount.Iban)
                throw new ValidationException("IBAN length is invalid.");

            var splitAccountNumberArray = Regex.Split(formattedAccountNumber, "(?<=\\G.{8})");

            var modResult = splitAccountNumberArray.Aggregate(string.Empty,
                (sum, number) => (long.Parse(sum + number) % 97).ToString());

            var checkDigits = (98 - int.Parse(modResult)).ToString();

            if (checkDigits.Length > 1)
                return checkDigits;

            return "0" + checkDigits;
        }

        public string GetIban(BankData bankData, string checkDigits, string branchCode, int nationalCheckDigit,
            string accountNumberText) =>
            $"{bankData.CountryCode}" +
            $"{checkDigits}" +
            $"{bankData.NationalBankCode}" +
            $"{branchCode}" +
            $"{nationalCheckDigit}" +
            $"{accountNumberText}";

        public string GetIbanSeparated(BankData bankData, string checkNumber, string branchCode, int nationalCheckDigit,
            string accountNumberText)
        {
            var splittedAccountNumberArray = Regex.Split(accountNumberText, "(?<=\\G.{4})")
                .Where(an => !string.IsNullOrEmpty(an)).ToArray();
            var separatedAccountNumber = string.Join(" ", splittedAccountNumberArray);

            return $"{bankData.CountryCode} " +
                   $"{checkNumber} " +
                   $"{bankData.NationalBankCode} " +
                   $"{branchCode}" +
                   $"{nationalCheckDigit} " +
                   $"{separatedAccountNumber}";
        }

        private BankData GetBankData() => _context.BankData.FirstOrDefault();

        private string GetBranchCode(int? branchId)
        {
            if (branchId == null)
            {
                var headquarters =
                    _context.Branches.SingleOrDefault(b => b.Id == _context.Headquarters.FirstOrDefault().Id);
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
            var maxAccountNumber = _context.BankAccounts.Max(b => (long?)b.AccountNumber);

            if (maxAccountNumber == null)
                return 0;

            return (long)(maxAccountNumber + 1);
        }

        private static string GetAccountNumberText(long accountNumber) => accountNumber.ToString("D16");

        private static void ThrowIfStringIsNotNumber(string input, string parameterName)
        {
            if (!Regex.IsMatch(input, @"^\d+$"))
                throw new ArgumentException("Parameter value is not a number.", nameof(parameterName));
        }
    }
}