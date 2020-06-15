using BankApp.Data;
using BankApp.Helpers;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BankApp.UnitTests.Helpers
{
    [TestClass]
    public class AccountNumberFactoryTests
    {
        private ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            return context;
        }

        private AccountNumberFactory accountNumberFactory;

        [TestInitialize]
        public void ClassInitalize()
        {
            accountNumberFactory = new AccountNumberFactory(GetMockContext());
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
    }
}
