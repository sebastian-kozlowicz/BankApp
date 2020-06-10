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
            var nationalBankCode = 1080;
            var branchCode = "000";
            var expectedNationalCheckDigit = 1;

            var result = accountNumberFactory.GenerateNationalCheckDigit(nationalBankCode, branchCode);

            Assert.AreEqual(expectedNationalCheckDigit, result);
        }

        [TestMethod]
        public void GenerateCheckNumbert_Should_ReturnValidCheckNumber()
        {
            var bankData = new BankData
            {
                CountryCode = "PL",
                NationalBankCode = 1080
            };

            var branchCode = "000";
            var nationalCheckDigit = 1;
            var accountNumberText = "9999999999999999";

            var result = accountNumberFactory.GenerateCheckNumber(bankData, branchCode, nationalCheckDigit, accountNumberText);

            Assert.AreEqual(1, result);
        }
    }
}
