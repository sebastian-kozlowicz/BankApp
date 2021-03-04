using System;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Helpers.Builders;
using BankApp.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace BankApp.UnitTests.Helpers.Builders
{
    [TestClass]
    public class MastercardPaymentCardNumberBuilderTests
    {
        private MastercardPaymentCardNumberBuilder _sut;
        private ApplicationDbContext _context;
        private readonly BankAccount _firstBankAccount = new BankAccount
        {
            Id = 1,
            AccountType = AccountType.Checking,
            Currency = Currency.Eur,
            CountryCode = "PL",
            CheckDigits = "61",
            NationalBankCode = "1080",
            BranchCode = "000",
            NationalCheckDigit = 1,
            AccountNumber = 0,
            AccountNumberText = "0000000000000000",
            Iban = "PL61108000010000000000000000",
            IbanSeparated = "PL 61 1080 0001 0000 0000 0000 0000",
            Balance = 0,
            DebitLimit = 0,
            CustomerId = 1,
            CreatedById = 1
        };

        private ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.BankIdentificationNumberData.Add(new BankIdentificationNumberData { Id = 1, BankIdentificationNumber = 510918, IssuingNetwork = IssuingNetwork.Mastercard });
            context.BankAccounts.Add(_firstBankAccount);
            context.Users.Add(new ApplicationUser { Id = 1, Customer = new Customer { Id = 1 } });
            context.SaveChanges();

            return context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = GetMockContext();
            _sut = new MastercardPaymentCardNumberBuilder(_context);
        }

        [TestMethod]
        public void GeneratePaymentCardNumber_Should_ReturnValidPaymentCardNumber()
        {
            // Arrange
            var expectedPaymentCardNumber = new PaymentCardNumber
            {
                MajorIndustryIdentifier = 5,
                BankIdentificationNumber = 510918,
                AccountIdentificationNumber = 0,
                AccountIdentificationNumberText = "000000000",
                CheckDigit = 9,
                Number = "5109180000000009",
                IssuingNetwork = IssuingNetwork.Mastercard
            };

            // Act
            var result = _sut.GeneratePaymentCardNumber(IssuingNetworkSettings.Mastercard.Length.Sixteen, _firstBankAccount.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedPaymentCardNumber);
        }
    }
}