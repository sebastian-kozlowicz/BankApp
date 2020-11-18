using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Controllers;
using BankApp.Data;
using BankApp.Dtos.Address;
using BankApp.Dtos.ApplicationUser;
using BankApp.Dtos.Auth;
using BankApp.Dtos.BankAccount;
using BankApp.Dtos.BankAccount.WithCustomerCreation;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Mapping;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BankAccountCreationDto = BankApp.Dtos.BankAccount.BankAccountCreationDto;

namespace BankApp.UnitTests.Controllers
{
    [TestClass]
    public class BankAccountsControllerTests
    {
        private BankAccountsController _bankAccountsController;
        private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        private Mock<IAccountNumberFactory> _accountNumberFactoryMock;
        private Mock<IUserStore<ApplicationUser>> _userStoreMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private ApplicationDbContext _context;

        private static ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.BankData.Add(new BankData { CountryCode = "PL", NationalBankCode = "1080" });
            context.Branches.Add(new Branch { Id = 1, BranchCode = "000" });
            context.Headquarters.Add(new Headquarters { Id = 1 });
            context.Users.Add(new ApplicationUser { Id = 1 });
            context.Customers.Add(new Customer { Id = 1 });
            context.SaveChanges();

            return context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = GetMockContext();
            _userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
            _accountNumberFactoryMock = new Mock<IAccountNumberFactory>();
            _bankAccountsController = new BankAccountsController(_userManagerMock.Object, _context, _mapper, _accountNumberFactoryMock.Object);
        }

        [TestMethod]
        public void CreateBankAccount_Should_CreateBankAccount_And_ReturnBankAccountDto_When_ModelStateIsValid()
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
            Assert.AreEqual(expectedBankAccount.CustomerId, bankAccountDto.CustomerId);

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

        [TestMethod]
        public void CreateBankAccount_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountCreationDto();
            _bankAccountsController.ModelState.AddModelError("Currency", "The Currency field is required.");
            _bankAccountsController.ModelState.AddModelError("CustomerId", "The CustomerId field is required.");
            _bankAccountsController.ModelState.AddModelError("AccountType", "The AccountType field is required.");

            // Act
            var badRequestResult = _bankAccountsController.CreateBankAccount(bankAccountCreation).Result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(badRequestResult);

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey("Currency"));
            Assert.IsTrue(error.ContainsKey("CustomerId"));
            Assert.IsTrue(error.ContainsKey("AccountType"));

            var currencyErrorValues = error["Currency"] as string[];
            Assert.IsNotNull(currencyErrorValues);
            Assert.IsTrue(currencyErrorValues.Single() == "The Currency field is required.");

            var customerIdErrorValues = error["CustomerId"] as string[];
            Assert.IsNotNull(customerIdErrorValues);
            Assert.IsTrue(customerIdErrorValues.Single() == "The CustomerId field is required.");

            var accountTypeErrorValues = error["AccountType"] as string[];
            Assert.IsNotNull(accountTypeErrorValues);
            Assert.IsTrue(accountTypeErrorValues.Single() == "The AccountType field is required.");
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByCustomer_Should_CreateBankAccountWithCustomer_And_ReturnBankAccountDto_When_ModelStateIsValid()
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

            var bankAccountCreation = new BankAccountWithCustomerCreationByCustomerDto
            {
                Register = new RegisterDto
                {
                    User = new ApplicationUserCreationBySameUserDto
                    {
                        Name = "John",
                        Surname = "Smith",
                        Email = "john@smith.com",
                        PhoneNumber = "123456789",
                        Password = "qwerty"
                    },
                    Address = new AddressCreationDto
                    {
                        Country = "United States",
                        City = "New York",
                        Street = "Glenwood Ave",
                        HouseNumber = "10",
                        ApartmentNumber = "11",
                        PostalCode = "10028"
                    }
                },
                BankAccount = new Dtos.BankAccount.WithCustomerCreation.BankAccountCreationDto
                {
                    AccountType = AccountType.Checking,
                    Currency = Currency.Eur
                }
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

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser, string>((user, password) =>
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                });

            _accountNumberFactoryMock.Setup(anf => anf.GenerateAccountNumber(null)).Returns(bankAccountNumber);

            // Act
            var result = await _bankAccountsController.CreateBankAccountWithCustomerByCustomer(bankAccountCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));

            var createdAtRouteResult = result.Result as CreatedAtRouteResult;

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
        }
    }
}