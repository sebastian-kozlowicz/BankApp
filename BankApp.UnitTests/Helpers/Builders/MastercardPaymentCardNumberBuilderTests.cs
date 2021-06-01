using System;
using System.Linq;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Exceptions;
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
        private readonly BankAccount _bankAccount = new()
        {
            Id = 1,
            CustomerId = 1,
            CreatedById = 1
        };

        private ApplicationDbContext _context;
        private MastercardPaymentCardNumberBuilder _sut;

        private ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.BankIdentificationNumberData.Add(new BankIdentificationNumberData
            { Id = 1, BankIdentificationNumber = 510918, IssuingNetwork = IssuingNetwork.Mastercard });
            context.BankAccounts.Add(_bankAccount);
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
        public void GeneratePaymentCardNumber_Should_ReturnPaymentCardNumber_When_ValidLengthPassed()
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
            var result =
                _sut.GeneratePaymentCardNumber(IssuingNetworkSettings.Mastercard.Length.Sixteen, _bankAccount.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedPaymentCardNumber);
        }

        [TestMethod]
        public void
            GeneratePaymentCardNumber_Should_ReturnPaymentCardNumberWithIteratedAccountIdentificationNumber_When_SomePaymentCardExistsInDb()
        {
            // Arrange
            _context.PaymentCards.Add(new PaymentCard());
            _context.SaveChanges();

            var expectedPaymentCardNumber = new PaymentCardNumber
            {
                MajorIndustryIdentifier = 5,
                BankIdentificationNumber = 510918,
                AccountIdentificationNumber = 1,
                AccountIdentificationNumberText = "000000001",
                CheckDigit = 7,
                Number = "5109180000000017",
                IssuingNetwork = IssuingNetwork.Mastercard
            };

            // Act
            var result =
                _sut.GeneratePaymentCardNumber(IssuingNetworkSettings.Mastercard.Length.Sixteen, _bankAccount.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedPaymentCardNumber);
        }

        [TestMethod]
        public void GeneratePaymentCardNumber_Should_ThrowArgumentException_When_BankAccountNotExist()
        {
            // Arrange
            var bankAccountId = 999;

            // Act
            Action act = () =>
                _sut.GeneratePaymentCardNumber(IssuingNetworkSettings.Mastercard.Length.Sixteen, bankAccountId);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage($"Bank account with id {bankAccountId} doesn't exist.");
        }

        [TestMethod]
        public void GeneratePaymentCardNumber_Should_ThrowArgumentException_When_InvalidLengthPassed()
        {
            // Act
            Action act = () => _sut.GeneratePaymentCardNumber(1, _bankAccount.Id);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Requested Mastercard payment card number length is invalid.");
        }

        [TestMethod]
        public void
            GeneratePaymentCardNumber_Should_ThrowInvalidDataInDatabaseException_When_BankIdentificationNumberDataNotExist()
        {
            // Arrange
            _context.BankIdentificationNumberData.RemoveRange(
                _context.BankIdentificationNumberData.FirstOrDefault(bin =>
                    bin.IssuingNetwork == IssuingNetwork.Mastercard));
            _context.SaveChanges();

            // Act
            Action act = () =>
                _sut.GeneratePaymentCardNumber(IssuingNetworkSettings.Mastercard.Length.Sixteen, _bankAccount.Id);

            // Assert
            act.Should().Throw<InvalidDataInDatabaseException>().WithMessage(
                $"Bank identification number data for {IssuingNetwork.Mastercard} issuing network doesn't exist in database.");
        }

        [TestMethod]
        public void
            GeneratePaymentCardNumber_Should_ThrowInvalidDataInDatabaseException_When_BankIdentificationNumberPrefixIsInvalid()
        {
            // Arrange
            _context.BankIdentificationNumberData.RemoveRange(
                _context.BankIdentificationNumberData.FirstOrDefault(bin =>
                    bin.IssuingNetwork == IssuingNetwork.Mastercard));
            _context.BankIdentificationNumberData.Add(new BankIdentificationNumberData
            { Id = 1, BankIdentificationNumber = 127329, IssuingNetwork = IssuingNetwork.Mastercard });
            _context.SaveChanges();

            // Act
            Action act = () =>
                _sut.GeneratePaymentCardNumber(IssuingNetworkSettings.Mastercard.Length.Sixteen, _bankAccount.Id);

            // Assert
            act.Should().Throw<InvalidDataInDatabaseException>()
                .WithMessage("Mastercard bank identification number found in database is invalid.");
        }

        [TestMethod]
        public void GenerateCheckDigit_Should_Return_ValidCheckDigit()
        {
            // Arrange
            var number = "7992739871";

            // Act
            var result = _sut.GenerateCheckDigit(number);

            // Assert
            result.Should().Be(3);
        }
    }
}