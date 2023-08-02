using System;
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
            // Arrange
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

            // Act
            var result = _sut.GenerateBankAccountNumber(1);

            // Assert
            result.Should().BeEquivalentTo(expectedBankAccountNumber);
        }

        [TestMethod]
        public void GenerateAccountNumber_Should_ReturnIbanWithHeadquartersBranchCode_When_NoBranchIdIsPassedToMethod()
        {
            // Arrange
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

            // Act
            var result = _sut.GenerateBankAccountNumber();

            // Assert
            result.Should().BeEquivalentTo(expectedBankAccountNumber);
        }

        [TestMethod]
        public void GenerateAccountNumber_Should_ReturnIbanWithIteratedAccountNumber_When_SomeAccountNumberExistsInDb()
        {
            // Arrange
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

            // Act
            var result = _sut.GenerateBankAccountNumber(1);

            // Assert
            result.Should().BeEquivalentTo(expectedBankAccountNumber);
        }

        [TestMethod]
        public void GenerateNationalCheckDigit_Should_ReturnValidNationalCheckDigit()
        {
            // Arrange
            var nationalBankCode = "1080";
            var branchCode = "000";
            var expectedNationalCheckDigit = 1;

            // Act
            var result = _sut.GenerateNationalCheckDigit(nationalBankCode, branchCode);

            // Assert
            result.Should().Be(expectedNationalCheckDigit);
        }

        [TestMethod]
        public void GenerateNationalCheckDigit_Should_ThrowException_When_PassedNationalBankCodeIsNotNumber()
        {
            // Arrange
            var nationalBankCode = "not a number";
            var branchCode = "000";

            // Act
            Action action = () => _sut.GenerateNationalCheckDigit(nationalBankCode, branchCode);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(nationalBankCode)}')");
        }

        [TestMethod]
        public void GenerateNationalCheckDigit_Should_ThrowException_When_PassedBranchCodeIsNotNumber()
        {
            // Arrange
            var nationalBankCode = "1080";
            var branchCode = "not a number";

            // Act
            Action action = () => _sut.GenerateNationalCheckDigit(nationalBankCode, branchCode);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(branchCode)}')");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateNationalCheckDigit_Should_ThrowException_When_PassedNationalBankCodeLengthIsIncorrect(
            string nationalBankCode)
        {
            // Arrange
            var branchCode = "000";

            // Act
            Action action = () => _sut.GenerateNationalCheckDigit(nationalBankCode, branchCode);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage(
                    $"National bank code should be length of {NumberLengthSettings.BankAccount.NationalBankCode} numbers.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateNationalCheckDigit_Should_ThrowException_When_PassedBranchCodeLengthIsIncorrect(
            string branchCode)
        {
            // Arrange
            var nationalBankCode = "1080";

            // Act
            Action action = () => _sut.GenerateNationalCheckDigit(nationalBankCode, branchCode);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage($"Branch code should be length of {NumberLengthSettings.BankAccount.BranchCode} numbers.");
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ReturnValidCheckDigits()
        {
            // Arrange
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var branchCode = "000";
            var nationalCheckDigit = 1;
            var accountNumberText = "9999999999999999";
            var expectedNationalCheckDigits = "63";

            // Act
            var result = _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            result.Should().Be(expectedNationalCheckDigits);
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ReturnValidCheckDigitsWithLeadingZero()
        {
            // Arrange
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "2490"
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";
            var expectedNationalCheckDigits = "04";

            // Act
            var result = _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            result.Should().Be(expectedNationalCheckDigits);
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedNationalBankCodeIsNotNumber()
        {
            // Arrange
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "not a number"
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";

            // Act
            Action action = () => _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(bankData.NationalBankCode)}')");
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedBranchCodeIsNotNumber()
        {
            // Arrange
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var branchCode = "not a number";
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";

            // Act
            Action action = () => _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(branchCode)}')");
        }

        [TestMethod]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedAccountNumberTextIsNotNumber()
        {
            // Arrange
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;
            var accountNumberText = "not a number";

            // Act
            Action action = () => _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter value is not a number. (Parameter '{nameof(accountNumberText)}')");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedNationalBankCodeLengthIsIncorrect(
            string nationalBankCode)
        {
            // Arrange
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = nationalBankCode
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";

            // Act
            Action action = () => _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage(
                    $"National bank code should be length of {NumberLengthSettings.BankAccount.NationalBankCode} numbers.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedBranchCodeLengthIsIncorrect(
            string branchCode)
        {
            // Arrange
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";

            // Act
            Action action = () => _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage(
                    $"Branch code should be length of {NumberLengthSettings.BankAccount.BranchCode} numbers.");
        }

        [DataTestMethod]
        [DataRow("0")]
        [DataRow("123456")]
        public void GenerateCheckDigits_Should_ThrowException_When_PassedAccountNumberTextLengthIsIncorrect(
            string accountNumberText)
        {
            // Arrange
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;

            // Act
            Action action = () => _sut.GenerateCheckDigits(bankData, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage(
                    $"Account number text should be length of {NumberLengthSettings.BankAccount.AccountNumber} numbers.");
        }

        [DataTestMethod]
        [DataRow("PL61108000010000000000000000", true)]
        [DataRow("PL27108000140000000000000000", true)]
        [DataRow("PL27108000140000000000000001", false)]
        public void ValidateBankAccountNumber_Should_ReturnExpectedResult(string iban, bool expectedResult)
        {
            // Act
            var result = _sut.ValidateBankAccountNumber(iban);

            // Assert
            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void GetIban_Should_ReturnIban()
        {
            // Arrange
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

            // Act
            var result = _sut.GetIban(bankData, checkDigits, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            result.Should().Be(expectedIban);
        }

        [TestMethod]
        public void GetSeparatedIban_Should_ReturnSeparatedIban()
        {
            // Arrange
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

            // Act
            var result =
                _sut.GetIbanSeparated(bankData, checkDigits, branchCode, nationalCheckDigit, accountNumberText);

            // Assert
            result.Should().Be(expectedIbanSeparated);
        }
    }
}