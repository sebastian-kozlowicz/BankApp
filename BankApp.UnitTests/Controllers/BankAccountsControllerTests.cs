using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Configuration;
using BankApp.Controllers;
using BankApp.Data;
using BankApp.Dtos.Address;
using BankApp.Dtos.ApplicationUser;
using BankApp.Dtos.Auth;
using BankApp.Dtos.BankAccount;
using BankApp.Dtos.BankAccount.WithCustomerCreation;
using BankApp.Dtos.Customer;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Mapping;
using BankApp.Models;
using Microsoft.AspNetCore.Http;
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
            context.Users.Add(new ApplicationUser { Id = 2 });
            context.Customers.Add(new Customer { Id = 1 });
            context.Tellers.Add(new Teller { Id = 2, WorkAtId = 1 });
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

            var expectedBankAccount = new BankAccountDto
            {
                AccountType = (AccountType)bankAccountCreation.AccountType,
                Currency = (Currency)bankAccountCreation.Currency,
                CountryCode = bankAccountNumber.CountryCode,
                CheckNumber = bankAccountNumber.CheckNumber,
                NationalBankCode = bankAccountNumber.NationalBankCode,
                BranchCode = bankAccountNumber.BranchCode,
                NationalCheckDigit = bankAccountNumber.NationalCheckDigit,
                AccountNumber = bankAccountNumber.AccountNumber,
                AccountNumberText = bankAccountNumber.AccountNumberText,
                Iban = bankAccountNumber.Iban,
                IbanSeparated = bankAccountNumber.IbanSeparated,
                CustomerId = (int)bankAccountCreation.CustomerId,
                CreatedById = (int)bankAccountCreation.CustomerId
            };

            _accountNumberFactoryMock.Setup(anf => anf.GenerateBankAccountNumber(null)).Returns(bankAccountNumber);

            // Act
            var createdAtRouteResult = _bankAccountsController.CreateBankAccount(bankAccountCreation).Result as CreatedAtRouteResult;

            // Assert
            Assert.IsNotNull(createdAtRouteResult);
            Assert.IsInstanceOfType(createdAtRouteResult.Value, typeof(BankAccountDto));

            var bankAccountDto = createdAtRouteResult.Value as BankAccountDto;
            Assert.IsNotNull(bankAccountDto);
            Assert.AreEqual(expectedBankAccount.AccountType, bankAccountDto.AccountType);
            Assert.AreEqual(expectedBankAccount.Currency, bankAccountDto.Currency);
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
            Assert.AreEqual(expectedBankAccount.CreatedById, bankAccountDto.CreatedById);

            var bankAccountFromDb = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountDto.Id);
            Assert.IsNotNull(bankAccountFromDb);
            Assert.AreEqual(expectedBankAccount.AccountType, bankAccountFromDb.AccountType);
            Assert.AreEqual(expectedBankAccount.Currency, bankAccountFromDb.Currency);
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
            Assert.AreEqual(expectedBankAccount.CreatedById, bankAccountFromDb.CreatedById);
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
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

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

            var expectedBankAccount = new BankAccountDto
            {
                AccountType = (AccountType)bankAccountCreation.BankAccount.AccountType,
                Currency = (Currency)bankAccountCreation.BankAccount.Currency,
                CountryCode = bankAccountNumber.CountryCode,
                CheckNumber = bankAccountNumber.CheckNumber,
                NationalBankCode = bankAccountNumber.NationalBankCode,
                BranchCode = bankAccountNumber.BranchCode,
                NationalCheckDigit = bankAccountNumber.NationalCheckDigit,
                AccountNumber = bankAccountNumber.AccountNumber,
                AccountNumberText = bankAccountNumber.AccountNumberText,
                Iban = bankAccountNumber.Iban,
                IbanSeparated = bankAccountNumber.IbanSeparated,
                Customer = new CustomerDto
                {
                    ApplicationUser = new ApplicationUserDto
                    {
                        Name = bankAccountCreation.Register.User.Name,
                        Surname = bankAccountCreation.Register.User.Surname,
                        Email = bankAccountCreation.Register.User.Email,
                        PhoneNumber = bankAccountCreation.Register.User.PhoneNumber
                    }
                }
            };

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser, string>((user, password) =>
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                });

            _accountNumberFactoryMock.Setup(anf => anf.GenerateBankAccountNumber(null)).Returns(bankAccountNumber);

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
            Assert.AreEqual(expectedBankAccount.AccountType, bankAccountDto.AccountType);
            Assert.AreEqual(expectedBankAccount.Currency, bankAccountDto.Currency);
            Assert.AreEqual(expectedBankAccount.CountryCode, bankAccountDto.CountryCode);
            Assert.AreEqual(expectedBankAccount.CheckNumber, bankAccountDto.CheckNumber);
            Assert.AreEqual(expectedBankAccount.NationalBankCode, bankAccountDto.NationalBankCode);
            Assert.AreEqual(expectedBankAccount.BranchCode, bankAccountDto.BranchCode);
            Assert.AreEqual(expectedBankAccount.NationalCheckDigit, bankAccountDto.NationalCheckDigit);
            Assert.AreEqual(expectedBankAccount.AccountNumber, bankAccountDto.AccountNumber);
            Assert.AreEqual(expectedBankAccount.AccountNumberText, bankAccountDto.AccountNumberText);
            Assert.AreEqual(expectedBankAccount.Iban, bankAccountDto.Iban);
            Assert.AreEqual(expectedBankAccount.IbanSeparated, bankAccountDto.IbanSeparated);
            Assert.AreEqual(bankAccountDto.CreatedById, bankAccountDto.Customer.Id);
            Assert.AreEqual(bankAccountDto.Customer.Id, bankAccountDto.Customer.ApplicationUser.Id);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Name, bankAccountDto.Customer.ApplicationUser.Name);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Surname, bankAccountDto.Customer.ApplicationUser.Surname);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Email, bankAccountDto.Customer.ApplicationUser.Email);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.PhoneNumber, bankAccountDto.Customer.ApplicationUser.PhoneNumber);

            var bankAccountFromDb = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountDto.Id);
            Assert.IsNotNull(bankAccountFromDb);
            Assert.AreEqual(expectedBankAccount.AccountType, bankAccountFromDb.AccountType);
            Assert.AreEqual(expectedBankAccount.Currency, bankAccountFromDb.Currency);
            Assert.AreEqual(expectedBankAccount.CountryCode, bankAccountFromDb.CountryCode);
            Assert.AreEqual(expectedBankAccount.CheckNumber, bankAccountFromDb.CheckNumber);
            Assert.AreEqual(expectedBankAccount.NationalBankCode, bankAccountFromDb.NationalBankCode);
            Assert.AreEqual(expectedBankAccount.BranchCode, bankAccountFromDb.BranchCode);
            Assert.AreEqual(expectedBankAccount.NationalCheckDigit, bankAccountFromDb.NationalCheckDigit);
            Assert.AreEqual(expectedBankAccount.AccountNumber, bankAccountFromDb.AccountNumber);
            Assert.AreEqual(expectedBankAccount.AccountNumberText, bankAccountFromDb.AccountNumberText);
            Assert.AreEqual(expectedBankAccount.Iban, bankAccountFromDb.Iban);
            Assert.AreEqual(expectedBankAccount.IbanSeparated, bankAccountFromDb.IbanSeparated);
            Assert.AreEqual(bankAccountFromDb.CreatedById, bankAccountFromDb.Customer.Id);
            Assert.AreEqual(bankAccountDto.Customer.Id, bankAccountDto.Customer.ApplicationUser.Id);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Name, bankAccountFromDb.Customer.ApplicationUser.Name);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Surname, bankAccountFromDb.Customer.ApplicationUser.Surname);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Email, bankAccountFromDb.Customer.ApplicationUser.Email);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.PhoneNumber, bankAccountFromDb.Customer.ApplicationUser.PhoneNumber);
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByCustomer_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountWithCustomerCreationByCustomerDto();
            _bankAccountsController.ModelState.AddModelError("Register", "The Register field is required.");
            _bankAccountsController.ModelState.AddModelError("BankAccount", "The BankAccount field is required.");

            // Act
            var result = await _bankAccountsController.CreateBankAccountWithCustomerByCustomer(bankAccountCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey("Register"));
            Assert.IsTrue(error.ContainsKey("BankAccount"));

            var currencyErrorValues = error["Register"] as string[];
            Assert.IsNotNull(currencyErrorValues);
            Assert.IsTrue(currencyErrorValues.Single() == "The Register field is required.");

            var customerIdErrorValues = error["BankAccount"] as string[];
            Assert.IsNotNull(customerIdErrorValues);
            Assert.IsTrue(customerIdErrorValues.Single() == "The BankAccount field is required.");
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByWorker_Should_CreateBankAccountWithCustomer_And_ReturnBankAccountDto_When_ModelStateIsValid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountWithCustomerCreationByWorkerDto
            {
                Register = new RegisterByAnotherUserDto
                {
                    User = new ApplicationUserCreationByAnotherUserDto
                    {
                        Name = "John",
                        Surname = "Smith",
                        Email = "john@smith.com",
                        PhoneNumber = "123456789"
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

            var expectedBankAccount = new BankAccountDto
            {
                AccountType = (AccountType)bankAccountCreation.BankAccount.AccountType,
                Currency = (Currency)bankAccountCreation.BankAccount.Currency,
                CountryCode = bankAccountNumber.CountryCode,
                CheckNumber = bankAccountNumber.CheckNumber,
                NationalBankCode = bankAccountNumber.NationalBankCode,
                BranchCode = bankAccountNumber.BranchCode,
                NationalCheckDigit = bankAccountNumber.NationalCheckDigit,
                AccountNumber = bankAccountNumber.AccountNumber,
                AccountNumberText = bankAccountNumber.AccountNumberText,
                Iban = bankAccountNumber.Iban,
                IbanSeparated = bankAccountNumber.IbanSeparated,
                Customer = new CustomerDto
                {
                    ApplicationUser = new ApplicationUserDto
                    {
                        Name = bankAccountCreation.Register.User.Name,
                        Surname = bankAccountCreation.Register.User.Surname,
                        Email = bankAccountCreation.Register.User.Email,
                        PhoneNumber = bankAccountCreation.Register.User.PhoneNumber
                    }
                }
            };

            var currentUser = new ApplicationUser { Id = 2 };
            var claims = new List<Claim> { new Claim(CustomClaimTypes.UserId, currentUser.Id.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            _userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(() => true);

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser>(user =>
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                });

            _accountNumberFactoryMock.Setup(anf => anf.GenerateBankAccountNumber(It.IsAny<int>())).Returns(bankAccountNumber);

            _bankAccountsController.ControllerContext = context;

            // Act
            var result = await _bankAccountsController.CreateBankAccountWithCustomerByWorker(bankAccountCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtRouteResult));

            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.IsInstanceOfType(createdAtRouteResult.Value, typeof(BankAccountDto));

            var bankAccountDto = createdAtRouteResult.Value as BankAccountDto;
            Assert.IsNotNull(bankAccountDto);
            Assert.AreEqual(expectedBankAccount.AccountType, bankAccountDto.AccountType);
            Assert.AreEqual(expectedBankAccount.Currency, bankAccountDto.Currency);
            Assert.AreEqual(expectedBankAccount.CountryCode, bankAccountDto.CountryCode);
            Assert.AreEqual(expectedBankAccount.CheckNumber, bankAccountDto.CheckNumber);
            Assert.AreEqual(expectedBankAccount.NationalBankCode, bankAccountDto.NationalBankCode);
            Assert.AreEqual(expectedBankAccount.BranchCode, bankAccountDto.BranchCode);
            Assert.AreEqual(expectedBankAccount.NationalCheckDigit, bankAccountDto.NationalCheckDigit);
            Assert.AreEqual(expectedBankAccount.AccountNumber, bankAccountDto.AccountNumber);
            Assert.AreEqual(expectedBankAccount.AccountNumberText, bankAccountDto.AccountNumberText);
            Assert.AreEqual(expectedBankAccount.Iban, bankAccountDto.Iban);
            Assert.AreEqual(expectedBankAccount.IbanSeparated, bankAccountDto.IbanSeparated);
            Assert.AreEqual(currentUser.Id, bankAccountDto.CreatedById);
            Assert.AreEqual(bankAccountDto.Customer.Id, bankAccountDto.Customer.ApplicationUser.Id);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Name, bankAccountDto.Customer.ApplicationUser.Name);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Surname, bankAccountDto.Customer.ApplicationUser.Surname);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Email, bankAccountDto.Customer.ApplicationUser.Email);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.PhoneNumber, bankAccountDto.Customer.ApplicationUser.PhoneNumber);

            var bankAccountFromDb = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountDto.Id);
            Assert.IsNotNull(bankAccountFromDb);
            Assert.AreEqual(expectedBankAccount.AccountType, bankAccountFromDb.AccountType);
            Assert.AreEqual(expectedBankAccount.Currency, bankAccountFromDb.Currency);
            Assert.AreEqual(expectedBankAccount.CountryCode, bankAccountFromDb.CountryCode);
            Assert.AreEqual(expectedBankAccount.CheckNumber, bankAccountFromDb.CheckNumber);
            Assert.AreEqual(expectedBankAccount.NationalBankCode, bankAccountFromDb.NationalBankCode);
            Assert.AreEqual(expectedBankAccount.BranchCode, bankAccountFromDb.BranchCode);
            Assert.AreEqual(expectedBankAccount.NationalCheckDigit, bankAccountFromDb.NationalCheckDigit);
            Assert.AreEqual(expectedBankAccount.AccountNumber, bankAccountFromDb.AccountNumber);
            Assert.AreEqual(expectedBankAccount.AccountNumberText, bankAccountFromDb.AccountNumberText);
            Assert.AreEqual(expectedBankAccount.Iban, bankAccountFromDb.Iban);
            Assert.AreEqual(expectedBankAccount.IbanSeparated, bankAccountFromDb.IbanSeparated);
            Assert.AreEqual(currentUser.Id, bankAccountFromDb.CreatedById);
            Assert.AreEqual(bankAccountFromDb.Customer.Id, bankAccountFromDb.Customer.ApplicationUser.Id);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Name, bankAccountFromDb.Customer.ApplicationUser.Name);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Surname, bankAccountFromDb.Customer.ApplicationUser.Surname);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Email, bankAccountFromDb.Customer.ApplicationUser.Email);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.PhoneNumber, bankAccountFromDb.Customer.ApplicationUser.PhoneNumber);
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByWorker_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountWithCustomerCreationByWorkerDto();
            _bankAccountsController.ModelState.AddModelError("Register", "The Register field is required.");
            _bankAccountsController.ModelState.AddModelError("BankAccount", "The BankAccount field is required.");

            // Act
            var result = await _bankAccountsController.CreateBankAccountWithCustomerByWorker(bankAccountCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey("Register"));
            Assert.IsTrue(error.ContainsKey("BankAccount"));

            var currencyErrorValues = error["Register"] as string[];
            Assert.IsNotNull(currencyErrorValues);
            Assert.IsTrue(currencyErrorValues.Single() == "The Register field is required.");

            var customerIdErrorValues = error["BankAccount"] as string[];
            Assert.IsNotNull(customerIdErrorValues);
            Assert.IsTrue(customerIdErrorValues.Single() == "The BankAccount field is required.");
        }
    }
}