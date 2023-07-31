using System;
using System.Threading.Tasks;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Helpers.Builders.Number;
using BankApp.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankApp.UnitTests.Helpers.Builders
{
    [TestClass]
    public class BankAccountNumberBuilderTests
    {
        private ApplicationDbContext _context;
        private BankAccountNumberBuilder _sut;

        private static ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.BankData.Add(new BankData { CountryCode = "PL", NationalBankCode = "1080" });
            context.Branches.Add(new Branch { Id = 1, BranchCode = "000" });
            context.Branches.Add(new Branch { Id = 2, BranchCode = "001" });
            context.Headquarters.Add(new Headquarters { Id = 2 });
            context.SaveChanges();

            return context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = GetMockContext();
            _sut = new BankAccountNumberBuilder(_context);
        }

        [TestMethod]
        public void GenerateAccountNumber_Should_ReturnIban()
        {
            var expectedBankAccountNumber = new BankAccountNumber
            {
                CountryCode = "PL",
                CheckDigits = "61",
                NationalBankCode = "1080",
                BranchCode = "000",
                NationalCheckDigit = 1,
                AccountNumber = 0,
                AccountNumberText = "0000000000000000",
                Iban = "PL61108000010000000000000000",
                IbanSeparated = "PL 61 1080 0001 0000 0000 0000 0000"
            };

            var result = _sut.GenerateBankAccountNumber(1);

            Assert.AreEqual(expectedBankAccountNumber.CountryCode, result.CountryCode);
            Assert.AreEqual(expectedBankAccountNumber.CheckDigits, result.CheckDigits);
            Assert.AreEqual(expectedBankAccountNumber.NationalBankCode, result.NationalBankCode);
            Assert.AreEqual(expectedBankAccountNumber.BranchCode, result.BranchCode);
            Assert.AreEqual(expectedBankAccountNumber.NationalCheckDigit, result.NationalCheckDigit);
            Assert.AreEqual(expectedBankAccountNumber.AccountNumber, result.AccountNumber);
            Assert.AreEqual(expectedBankAccountNumber.AccountNumberText, result.AccountNumberText);
            Assert.AreEqual(expectedBankAccountNumber.Iban, result.Iban);
            Assert.AreEqual(expectedBankAccountNumber.IbanSeparated, result.IbanSeparated);
        }

        [TestMethod]
        public void GenerateAccountNumber_Should_ReturnIbanWithHeadquartersBranchCode_When_NoBranchIdIsPassedToMethod()
        {
            var expectedBankAccountNumber = new BankAccountNumber
            {
                CountryCode = "PL",
                CheckDigits = "27",
                NationalBankCode = "1080",
                BranchCode = "001",
                NationalCheckDigit = 4,
                AccountNumber = 0,
                AccountNumberText = "0000000000000000",
                Iban = "PL27108000140000000000000000",
                IbanSeparated = "PL 27 1080 0014 0000 0000 0000 0000"
            };

            var result = _sut.GenerateBankAccountNumber();

            Assert.AreEqual(expectedBankAccountNumber.CountryCode, result.CountryCode);
            Assert.AreEqual(expectedBankAccountNumber.CheckDigits, result.CheckDigits);
            Assert.AreEqual(expectedBankAccountNumber.NationalBankCode, result.NationalBankCode);
            Assert.AreEqual(expectedBankAccountNumber.BranchCode, result.BranchCode);
            Assert.AreEqual(expectedBankAccountNumber.NationalCheckDigit, result.NationalCheckDigit);
            Assert.AreEqual(expectedBankAccountNumber.AccountNumber, result.AccountNumber);
            Assert.AreEqual(expectedBankAccountNumber.AccountNumberText, result.AccountNumberText);
            Assert.AreEqual(expectedBankAccountNumber.Iban, result.Iban);
            Assert.AreEqual(expectedBankAccountNumber.IbanSeparated, result.IbanSeparated);
        }

        [TestMethod]
        public void GenerateAccountNumber_Should_ReturnIbanWithIteratedAccountNumber_When_SomeAccountNumberExistsInDb()
        {
            _context.BankAccounts.Add(new BankAccount { AccountNumber = 0 });
            _context.SaveChanges();

            var expectedBankAccountNumber = new BankAccountNumber
            {
                CountryCode = "PL",
                CheckDigits = "34",
                NationalBankCode = "1080",
                BranchCode = "000",
                NationalCheckDigit = 1,
                AccountNumber = 1,
                AccountNumberText = "0000000000000001",
                Iban = "PL34108000010000000000000001",
                IbanSeparated = "PL 34 1080 0001 0000 0000 0000 0001"
            };

            var result = _sut.GenerateBankAccountNumber(1);

            Assert.AreEqual(expectedBankAccountNumber.CountryCode, result.CountryCode);
            Assert.AreEqual(expectedBankAccountNumber.CheckDigits, result.CheckDigits);
            Assert.AreEqual(expectedBankAccountNumber.NationalBankCode, result.NationalBankCode);
            Assert.AreEqual(expectedBankAccountNumber.BranchCode, result.BranchCode);
            Assert.AreEqual(expectedBankAccountNumber.NationalCheckDigit, result.NationalCheckDigit);
            Assert.AreEqual(expectedBankAccountNumber.AccountNumber, result.AccountNumber);
            Assert.AreEqual(expectedBankAccountNumber.AccountNumberText, result.AccountNumberText);
            Assert.AreEqual(expectedBankAccountNumber.Iban, result.Iban);
            Assert.AreEqual(expectedBankAccountNumber.IbanSeparated, result.IbanSeparated);
        }

        [TestMethod]
        public void GenerateNationalCheckDigit_Should_ReturnValidNationalCheckDigit()
        {
            var nationalBankCode = "1080";
            var branchCode = "000";
            var expectedNationalCheckDigit = 1;

            var result = _sut.GenerateNationalCheckDigit(nationalBankCode, branchCode);

            Assert.AreEqual(expectedNationalCheckDigit, result);
        }

        [TestMethod]
        public void GenerateNationalCheckDigit_Should_ThrowException_When_PassedNationalBankCodeIsNotNumber()
        {
            var nationalBankCode = "not a number";
            var branchCode = "000";

            Func<Task> func = () => Task.FromResult(_sut.GenerateNationalCheckDigit(nationalBankCode, branchCode));

            func.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(nationalBankCode)}')");
        }

        [TestMethod]
        public void GenerateNationalCheckDigit_Should_ThrowException_When_PassedBranchCodeIsNotNumber()
        {
            var nationalBankCode = "1080";
            var branchCode = "not a number";

            Func<Task> func = () => Task.FromResult(_sut.GenerateNationalCheckDigit(nationalBankCode, branchCode));

            func.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(branchCode)}')");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateNationalCheckDigit_Should_ThrowException_When_PassedNationalBankCodeLengthIsIncorrect(
            string nationalBankCode)
        {
            var branchCode = "000";

            Func<Task> func = () => Task.FromResult(_sut.GenerateNationalCheckDigit(nationalBankCode, branchCode));

            func.Should().Throw<ArgumentException>()
                .WithMessage(
                    $"National bank code should be length of {NumberLengthSettings.BankAccount.NationalBankCode} numbers.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateNationalCheckDigit_Should_ThrowException_When_PassedBranchCodeLengthIsIncorrect(
            string branchCode)
        {
            var nationalBankCode = "1080";

            Func<Task> func = () => Task.FromResult(_sut.GenerateNationalCheckDigit(nationalBankCode, branchCode));

            func.Should().Throw<ArgumentException>()
                .WithMessage($"Branch code should be length of {NumberLengthSettings.BankAccount.BranchCode} numbers.");
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ReturnValidCheckDigits()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var branchCode = "000";
            var nationalCheckDigit = 1;
            var accountNumberText = "9999999999999999";
            var expectedNationalCheckDigits = "63";

            var result = _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            Assert.AreEqual(expectedNationalCheckDigits, result);
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ReturnValidCheckDigitsWithLeadingZero()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "2490"
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";
            var expectedNationalCheckDigits = "04";

            var result = _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            Assert.AreEqual(expectedNationalCheckDigits, result);
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedNationalBankCodeIsNotNumber()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "not a number"
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";

            Func<Task> func = () =>
                Task.FromResult(_sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText));

            func.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(bankData.NationalBankCode)}')");
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedBranchCodeIsNotNumber()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var branchCode = "not a number";
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";

            Func<Task> func = () =>
                Task.FromResult(_sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText));

            func.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(branchCode)}')");
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedAccountNumberTextIsNotNumber()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;
            var accountNumberText = "not a number";

            Func<Task> func = () =>
                Task.FromResult(_sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText));

            func.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(accountNumberText)}')");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedNationalBankCodeLengthIsIncorrect(
            string nationalBankCode)
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = nationalBankCode
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";

            Func<Task> func = () =>
                Task.FromResult(_sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText));

            func.Should().Throw<ArgumentException>()
                .WithMessage(
                    $"National bank code should be length of {NumberLengthSettings.BankAccount.NationalBankCode} numbers.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedBranchCodeLengthIsIncorrect(
            string branchCode)
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";

            Func<Task> func = () =>
                Task.FromResult(_sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText));

            func.Should().Throw<ArgumentException>()
                .WithMessage(
                    $"Branch code should be length of {NumberLengthSettings.BankAccount.BranchCode} numbers.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedAccountNumberTextLengthIsIncorrect(
            string accountNumberText)
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;

            Func<Task> func = () =>
                Task.FromResult(_sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText));

            func.Should().Throw<ArgumentException>()
                .WithMessage(
                    $"Account number text should be length of {NumberLengthSettings.BankAccount.AccountNumber} numbers.");
        }

        [TestMethod]
        public void GetIban_Should_ReturnIban()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var checkDigits = "63";
            var branchCode = "000";
            var nationalCheckDigit = 1;
            var accountNumberText = "9999999999999999";
            var expectedIban = "PL63108000019999999999999999";

            var result = _sut.GetIban(bankData, checkDigits, branchCode, nationalCheckDigit, accountNumberText);

            Assert.AreEqual(expectedIban, result);
        }

        [TestMethod]
        public void GetSeparatedIban_Should_ReturnSeparatedIban()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var checkDigits = "63";
            var branchCode = "000";
            var nationalCheckDigit = 1;
            var accountNumberText = "9999999999999999";
            var expectedIbanSeparated = "PL 63 1080 0001 9999 9999 9999 9999";

            var result =
                _sut.GetIbanSeparated(bankData, checkDigits, branchCode, nationalCheckDigit, accountNumberText);

            Assert.AreEqual(expectedIbanSeparated, result);
        }
    }
}