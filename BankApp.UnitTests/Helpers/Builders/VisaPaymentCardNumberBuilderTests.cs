using System;
using System.Collections.Generic;
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
    public class VisaPaymentCardNumberBuilderTests
    {
        private readonly BankAccount _bankAccount = new()
        {
            Id = 1,
            AccountNumber = 12000000000000,
            AccountNumberText = "0012000000000000",
            CustomerId = 1,
            CreatedById = 1
        };

        private ApplicationDbContext _context;
        private VisaPaymentCardNumberBuilder _sut;

        public static IEnumerable<object[]> PaymentCardNumberTestData
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        IssuingNetworkSettings.Visa.Length.Thirteen,
                        new PaymentCardNumber
                        {
                            MajorIndustryIdentifier = 4,
                            BankIdentificationNumber = 427329,
                            AccountIdentificationNumber = 1200,
                            AccountIdentificationNumberText = "001200",
                            CheckDigit = 3,
                            Number = "4273290012003",
                            IssuingNetwork = IssuingNetwork.Visa
                        }
                    },
                    new object[]
                    {
                        IssuingNetworkSettings.Visa.Length.Sixteen,
                        new PaymentCardNumber
                        {
                            MajorIndustryIdentifier = 4,
                            BankIdentificationNumber = 427329,
                            AccountIdentificationNumber = 1200000,
                            AccountIdentificationNumberText = "001200000",
                            CheckDigit = 5,
                            Number = "4273290012000005",
                            IssuingNetwork = IssuingNetwork.Visa
                        }
                    }
                };
            }
        }

        private ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.BankIdentificationNumberData.Add(new BankIdentificationNumberData { Id = 1, BankIdentificationNumber = 427329, IssuingNetwork = IssuingNetwork.Visa });
            context.BankAccounts.Add(_bankAccount);
            context.Users.Add(new ApplicationUser { Id = 1, Customer = new Customer { Id = 1 } });
            context.SaveChanges();

            return context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = GetMockContext();
            _sut = new VisaPaymentCardNumberBuilder(_context);
        }

        [TestMethod]
        [DynamicData(nameof(PaymentCardNumberTestData))]
        public void GeneratePaymentCardNumber_Should_ReturnPaymentCardNumber_When_ValidLengthPassed(int length, PaymentCardNumber expectedPaymentCardNumber)
        {
            // Act
            var result = _sut.GeneratePaymentCardNumber(length, _bankAccount.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedPaymentCardNumber);
        }
    }
}