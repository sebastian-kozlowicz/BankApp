using BankApp.Data;
using BankApp.Helpers;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BankApp.Tests.Helpers
{
    [TestClass]
    public class AccountNumberFactoryTests
    {
        private AccountNumberFactory accountNumberFactory;

        private ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.BankData.Add(new BankData { CountryCode = "PL", NationalBankCode = "1080" });
            context.Branches.Add(new Branch { Id = "1", BranchCode = "000" });
            context.SaveChanges();

            return context;
        }


        [TestInitialize]
        public void ClassInitalize()
        {
            accountNumberFactory = new AccountNumberFactory(GetMockContext());
        }

        [TestMethod]
        public void GenerateAccountNumber_Should_ReturnIban()
        {
            var expectedBankAccountNumber = new BankAccountNumber
            {
                CountryCode = "PL",
                CheckNumber = "61",
                NationalBankCode = "1080",
                BranchCode = "000",
                NationalCheckDigit = 1,
                AccountNumber = 0,
                AccountNumberText = "0000000000000000",
                Iban = "PL61108000010000000000000000",
                IbanSeparated = "PL 61 1080 0001 0000 0000 0000 0000"

            };

            var result = accountNumberFactory.GenerateAccountNumber("1");

            Assert.AreEqual(expectedBankAccountNumber.CountryCode, result.CountryCode);
            Assert.AreEqual(expectedBankAccountNumber.CheckNumber, result.CheckNumber);
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

            var result = accountNumberFactory.GenerateNationalCheckDigit(nationalBankCode, branchCode);

            Assert.AreEqual(expectedNationalCheckDigit, result);
        }

        [TestMethod]
        public void GenerateCheckNumber_Should_ReturnValidCheckNumber()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var branchCode = "000";
            var nationalCheckDigit = 1;
            var accountNumberText = "9999999999999999";
            var expectedNationalCheckNumber = "63";

            var result = accountNumberFactory.GenerateCheckNumber(bankData, branchCode, nationalCheckDigit, accountNumberText);

            Assert.AreEqual(expectedNationalCheckNumber, result);
        }

        [TestMethod]
        public void GenerateCheckNumber_Should_ReturnValidCheckNumberWithLeadingZero()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "2490"
            };
            var branchCode = "405";
            var nationalCheckDigit = 8;
            var accountNumberText = "8540304041736354";
            string expectedNationalCheckNumber = "04";

            var result = accountNumberFactory.GenerateCheckNumber(bankData, branchCode, nationalCheckDigit, accountNumberText);

            Assert.AreEqual(expectedNationalCheckNumber, result);
        }

        [TestMethod]
        public void GetIban_Should_ReturnIban()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var checkNumber = "63";
            var branchCode = "000";
            var nationalCheckDigit = 1;
            var accountNumberText = "9999999999999999";
            var expectedIban = "PL63108000019999999999999999";

            var result = accountNumberFactory.GetIban(bankData, checkNumber, branchCode, nationalCheckDigit, accountNumberText);

            Assert.AreEqual(expectedIban, result);
        }

        [TestMethod]
        public void GetSeparatedIban_Should_ReturnSeparatedIbanText()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = "1080"
            };
            var checkNumber = "63";
            var branchCode = "000";
            var nationalCheckDigit = 1;
            var accountNumberText = "9999999999999999";
            var expectedIbanSeparated = "PL 63 1080 0001 9999 9999 9999 9999";

            var result = accountNumberFactory.GetIbanSeparated(bankData, checkNumber, branchCode, nationalCheckDigit, accountNumberText);

            Assert.AreEqual(expectedIbanSeparated, result);
        }
    }
}
