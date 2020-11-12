using System;
using System.Linq;
using AutoMapper;
using BankApp.Controllers;
using BankApp.Data;
using BankApp.Dtos.BankAccount;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Mapping;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BankApp.UnitTests.Controllers
{
    [TestClass]
    public class BankAccountsControllerTests
    {
        private BankAccountsController _bankAccountsController;
        private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        private Mock<IAccountNumberFactory> _accountNumberFactoryMock;
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private static ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.BankData.Add(new BankData { CountryCode = "PL", NationalBankCode = "1080" });
            context.Branches.Add(new Branch { Id = 1, BranchCode = "000" });
            context.Headquarters.Add(new Headquarters { Id = 1 });
            context.Customers.Add(new Customer { Id = 1 });
            context.SaveChanges();

            return context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = GetMockContext();
            _accountNumberFactoryMock = new Mock<IAccountNumberFactory>();
            _bankAccountsController = new BankAccountsController(_userManager, _context, _mapper, _accountNumberFactoryMock.Object);
        }

        [TestMethod]
        public void CreateBankAccount_Should_CreateBankAccount_And_ReturnBankAccountDto()
        {
            // Arrange
            var expectedBankAccount = new BankAccount
            {
                AccountType = AccountType.Checking,
                Currency = Currency.Eur,
                CountryCode = "PL",
                CheckNumber = "61",
                NationalBankCode = "1080",
                BranchCode = "000",
                NationalCheckDigit = 1,
                AccountNumber = 0,
                AccountNumberText = "0000000000000000",
                Iban = "PL61108000010000000000000000",
                IbanSeparated = "PL 61 1080 0001 0000 0000 0000 0000",
                CustomerId = 1
            };

            var bankAccountCreation = new BankAccountCreationDto
            {
                AccountType = AccountType.Checking, 
                Currency = Currency.Eur, 
                CustomerId = 1
            };

            var bankAccountNumber = new BankAccountNumber
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

            _accountNumberFactoryMock.Setup(anf => anf.GenerateAccountNumber(null)).Returns(bankAccountNumber);

            // Act
            var createdAtRouteResult = _bankAccountsController.CreateBankAccount(bankAccountCreation).Result as CreatedAtRouteResult;

            // Assert
            Assert.IsNotNull(createdAtRouteResult);
            Assert.IsInstanceOfType(createdAtRouteResult.Value, typeof(BankAccountDto));

            var bankAccountDto = createdAtRouteResult.Value as BankAccountDto;

            Assert.IsNotNull(bankAccountDto);
            Assert.AreEqual(expectedBankAccount.CountryCode, bankAccountDto.CountryCode);
            Assert.AreEqual(expectedBankAccount.CheckNumber, bankAccountDto.CheckNumber);
            Assert.AreEqual(expectedBankAccount.NationalBankCode, bankAccountDto.NationalBankCode);
            Assert.AreEqual(expectedBankAccount.BranchCode, bankAccountDto.BranchCode);
            Assert.AreEqual(expectedBankAccount.NationalCheckDigit, bankAccountDto.NationalCheckDigit);
            Assert.AreEqual(expectedBankAccount.AccountNumber, bankAccountDto.AccountNumber);
            Assert.AreEqual(expectedBankAccount.AccountNumberText, bankAccountDto.AccountNumberText);
            Assert.AreEqual(expectedBankAccount.Iban, bankAccountDto.Iban);
            Assert.AreEqual(expectedBankAccount.IbanSeparated, bankAccountDto.IbanSeparated);
            //Assert.AreEqual(expectedBankAccount.CustomerId, bankAccountDto.CustomerId);

            var bankAccountFromDb = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountDto.Id);

            Assert.IsNotNull(bankAccountFromDb);
            Assert.AreEqual(expectedBankAccount.CountryCode, bankAccountFromDb.CountryCode);
            Assert.AreEqual(expectedBankAccount.CheckNumber, bankAccountFromDb.CheckNumber);
            Assert.AreEqual(expectedBankAccount.NationalBankCode, bankAccountFromDb.NationalBankCode);
            Assert.AreEqual(expectedBankAccount.BranchCode, bankAccountFromDb.BranchCode);
            Assert.AreEqual(expectedBankAccount.NationalCheckDigit, bankAccountFromDb.NationalCheckDigit);
            Assert.AreEqual(expectedBankAccount.AccountNumber, bankAccountFromDb.AccountNumber);
            Assert.AreEqual(expectedBankAccount.AccountNumberText, bankAccountFromDb.AccountNumberText);
            Assert.AreEqual(expectedBankAccount.Iban, bankAccountFromDb.Iban);
            Assert.AreEqual(expectedBankAccount.IbanSeparated, bankAccountFromDb.IbanSeparated);
            Assert.AreEqual(expectedBankAccount.CustomerId, bankAccountFromDb.CustomerId);
        }
    }
}
