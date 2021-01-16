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
        private BankAccountsController _sut;
        private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        private Mock<IBankAccountNumberBuilder> _bankAccountNumberBuilderMock;
        private Mock<IUserStore<ApplicationUser>> _userStoreMock;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private ApplicationDbContext _context;
        private readonly BankAccount _firstBankAccount = new BankAccount
        {
            Id = 1,
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
            Balance = 0,
            DebitLimit = 0,
            CustomerId = 1,
            CreatedById = 1
        };
        private readonly BankAccount _secondBankAccount = new BankAccount
        {
            Id = 2,
            AccountType = AccountType.Savings,
            Currency = Currency.Pln,
            CountryCode = "PL",
            CheckNumber = "27",
            NationalBankCode = "1080",
            BranchCode = "001",
            NationalCheckDigit = 4,
            AccountNumber = 0,
            AccountNumberText = "0000000000000000",
            Iban = "PL27108000140000000000000000",
            IbanSeparated = "PL 27 1080 0014 0000 0000 0000 0000",
            Balance = 0,
            DebitLimit = 0,
            CustomerId = 1,
            CreatedById = 1
        };
        private IEnumerable<BankAccount> BankAccounts => new List<BankAccount> { _firstBankAccount, _secondBankAccount };

        private ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.BankData.Add(new BankData { CountryCode = "PL", NationalBankCode = "1080" });
            context.Branches.Add(new Branch { Id = 1, BranchCode = "000" });
            context.Headquarters.Add(new Headquarters { Id = 1 });
            context.BankAccounts.AddRange(BankAccounts);
            context.Users.Add(new ApplicationUser { Id = 1, Customer = new Customer { Id = 1 } });
            context.Users.Add(new ApplicationUser { Id = 2, Teller = new Teller { Id = 2, WorkAtId = 1 } });
            context.Users.Add(new ApplicationUser { Id = 3, Teller = new Teller { Id = 3 } });
            context.Users.Add(new ApplicationUser { Id = 4, Manager = new Manager { Id = 4 } });
            context.SaveChanges();

            return context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = GetMockContext();
            _userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
            _bankAccountNumberBuilderMock = new Mock<IBankAccountNumberBuilder>();
            _sut = new BankAccountsController(_userManagerMock.Object, _context, _mapper, _bankAccountNumberBuilderMock.Object);
        }

        [TestMethod]
        public void GetBankAccount_Should_ReturnBankAccountDto_When_BankAccountIsFound()
        {
            var okResult = _sut.GetBankAccount(1).Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(BankAccountDto));

            var bankAccountDto = okResult.Value as BankAccountDto;
            Assert.IsNotNull(bankAccountDto);
            Assert.AreEqual(_firstBankAccount.Id, bankAccountDto.Id);
            Assert.AreEqual(_firstBankAccount.AccountType, bankAccountDto.AccountType);
            Assert.AreEqual(_firstBankAccount.Currency, bankAccountDto.Currency);
            Assert.AreEqual(_firstBankAccount.CountryCode, bankAccountDto.CountryCode);
            Assert.AreEqual(_firstBankAccount.CheckNumber, bankAccountDto.CheckNumber);
            Assert.AreEqual(_firstBankAccount.NationalBankCode, bankAccountDto.NationalBankCode);
            Assert.AreEqual(_firstBankAccount.BranchCode, bankAccountDto.BranchCode);
            Assert.AreEqual(_firstBankAccount.NationalCheckDigit, bankAccountDto.NationalCheckDigit);
            Assert.AreEqual(_firstBankAccount.AccountNumber, bankAccountDto.AccountNumber);
            Assert.AreEqual(_firstBankAccount.AccountNumberText, bankAccountDto.AccountNumberText);
            Assert.AreEqual(_firstBankAccount.Iban, bankAccountDto.Iban);
            Assert.AreEqual(_firstBankAccount.IbanSeparated, bankAccountDto.IbanSeparated);
            Assert.AreEqual(_firstBankAccount.Balance, bankAccountDto.Balance);
            Assert.AreEqual(_firstBankAccount.DebitLimit, bankAccountDto.DebitLimit);
            Assert.AreEqual(_firstBankAccount.CustomerId, bankAccountDto.CustomerId);
            Assert.AreEqual(_firstBankAccount.CreatedById, bankAccountDto.CreatedById);
        }

        [TestMethod]
        public void GetBankAccount_Should_ReturnNotFound_When_BankAccountNotFound()
        {
            var notFoundResult = _sut.GetBankAccount(999);

            Assert.IsNotNull(notFoundResult);
            Assert.IsInstanceOfType(notFoundResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetBankAccountsForUser_Should_ReturnBankAccountDtoList_When_BankAccountsAreFound()
        {
            var okResult = _sut.GetBankAccountsForUser(1).Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(List<BankAccountDto>));

            var bankAccountsDto = okResult.Value as List<BankAccountDto>;
            Assert.IsNotNull(bankAccountsDto);
            CollectionAssert.AllItemsAreNotNull(bankAccountsDto);

            Assert.AreEqual(_firstBankAccount.Id, bankAccountsDto[0].Id);
            Assert.AreEqual(_firstBankAccount.AccountType, bankAccountsDto[0].AccountType);
            Assert.AreEqual(_firstBankAccount.Currency, bankAccountsDto[0].Currency);
            Assert.AreEqual(_firstBankAccount.CountryCode, bankAccountsDto[0].CountryCode);
            Assert.AreEqual(_firstBankAccount.CheckNumber, bankAccountsDto[0].CheckNumber);
            Assert.AreEqual(_firstBankAccount.NationalBankCode, bankAccountsDto[0].NationalBankCode);
            Assert.AreEqual(_firstBankAccount.BranchCode, bankAccountsDto[0].BranchCode);
            Assert.AreEqual(_firstBankAccount.NationalCheckDigit, bankAccountsDto[0].NationalCheckDigit);
            Assert.AreEqual(_firstBankAccount.AccountNumber, bankAccountsDto[0].AccountNumber);
            Assert.AreEqual(_firstBankAccount.AccountNumberText, bankAccountsDto[0].AccountNumberText);
            Assert.AreEqual(_firstBankAccount.Iban, bankAccountsDto[0].Iban);
            Assert.AreEqual(_firstBankAccount.IbanSeparated, bankAccountsDto[0].IbanSeparated);
            Assert.AreEqual(_firstBankAccount.Balance, bankAccountsDto[0].Balance);
            Assert.AreEqual(_firstBankAccount.DebitLimit, bankAccountsDto[0].DebitLimit);
            Assert.AreEqual(_firstBankAccount.CustomerId, bankAccountsDto[0].CustomerId);
            Assert.AreEqual(_firstBankAccount.CreatedById, bankAccountsDto[0].CreatedById);

            Assert.AreEqual(_secondBankAccount.Id, bankAccountsDto[1].Id);
            Assert.AreEqual(_secondBankAccount.AccountType, bankAccountsDto[1].AccountType);
            Assert.AreEqual(_secondBankAccount.Currency, bankAccountsDto[1].Currency);
            Assert.AreEqual(_secondBankAccount.CountryCode, bankAccountsDto[1].CountryCode);
            Assert.AreEqual(_secondBankAccount.CheckNumber, bankAccountsDto[1].CheckNumber);
            Assert.AreEqual(_secondBankAccount.NationalBankCode, bankAccountsDto[1].NationalBankCode);
            Assert.AreEqual(_secondBankAccount.BranchCode, bankAccountsDto[1].BranchCode);
            Assert.AreEqual(_secondBankAccount.NationalCheckDigit, bankAccountsDto[1].NationalCheckDigit);
            Assert.AreEqual(_secondBankAccount.AccountNumber, bankAccountsDto[1].AccountNumber);
            Assert.AreEqual(_secondBankAccount.AccountNumberText, bankAccountsDto[1].AccountNumberText);
            Assert.AreEqual(_secondBankAccount.Iban, bankAccountsDto[1].Iban);
            Assert.AreEqual(_secondBankAccount.IbanSeparated, bankAccountsDto[1].IbanSeparated);
            Assert.AreEqual(_secondBankAccount.Balance, bankAccountsDto[1].Balance);
            Assert.AreEqual(_secondBankAccount.DebitLimit, bankAccountsDto[1].DebitLimit);
            Assert.AreEqual(_secondBankAccount.CustomerId, bankAccountsDto[1].CustomerId);
            Assert.AreEqual(_secondBankAccount.CreatedById, bankAccountsDto[1].CreatedById);
        }

        [TestMethod]
        public void GetBankAccountsForUser_Should_ReturnNotFound_When_BankAccountsNotFound()
        {
            var notFoundResult = _sut.GetBankAccountsForUser(999);

            Assert.IsNotNull(notFoundResult);
            Assert.IsInstanceOfType(notFoundResult.Result, typeof(NotFoundResult));
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
                Balance = 0,
                DebitLimit = 0,
                CustomerId = (int)bankAccountCreation.CustomerId,
                CreatedById = (int)bankAccountCreation.CustomerId
            };

            _bankAccountNumberBuilderMock.Setup(anf => anf.GenerateBankAccountNumber(null)).Returns(bankAccountNumber);

            // Act
            var createdAtRouteResult = _sut.CreateBankAccount(bankAccountCreation).Result as CreatedAtRouteResult;

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
            Assert.AreEqual(expectedBankAccount.Balance, bankAccountDto.Balance);
            Assert.AreEqual(expectedBankAccount.DebitLimit, bankAccountDto.DebitLimit);
            Assert.AreNotEqual(DateTime.MinValue, bankAccountDto.OpenedDate);
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
            Assert.AreEqual(expectedBankAccount.Balance, bankAccountFromDb.Balance);
            Assert.AreEqual(expectedBankAccount.DebitLimit, bankAccountFromDb.DebitLimit);
            Assert.AreNotEqual(DateTime.MinValue, bankAccountFromDb.OpenedDate);
            Assert.AreEqual(expectedBankAccount.CustomerId, bankAccountFromDb.CustomerId);
            Assert.AreEqual(expectedBankAccount.CreatedById, bankAccountFromDb.CreatedById);
        }

        [TestMethod]
        public void CreateBankAccount_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountCreationDto();
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.AccountType), $"The {nameof(bankAccountCreation.AccountType)} field is required.");
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.Currency), $"The {nameof(bankAccountCreation.Currency)} field is required.");
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.CustomerId), $"The {nameof(bankAccountCreation.CustomerId)} field is required.");

            // Act
            var badRequestResult = _sut.CreateBankAccount(bankAccountCreation).Result as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey(nameof(bankAccountCreation.AccountType)));
            Assert.IsTrue(error.ContainsKey(nameof(bankAccountCreation.Currency)));
            Assert.IsTrue(error.ContainsKey(nameof(bankAccountCreation.CustomerId)));

            var currencyErrorValues = error[nameof(bankAccountCreation.AccountType)] as string[];
            Assert.IsNotNull(currencyErrorValues);
            Assert.IsTrue(currencyErrorValues.Single() == $"The {nameof(bankAccountCreation.AccountType)} field is required.");

            var customerIdErrorValues = error[nameof(bankAccountCreation.Currency)] as string[];
            Assert.IsNotNull(customerIdErrorValues);
            Assert.IsTrue(customerIdErrorValues.Single() == $"The {nameof(bankAccountCreation.Currency)} field is required.");

            var accountTypeErrorValues = error[nameof(bankAccountCreation.CustomerId)] as string[];
            Assert.IsNotNull(accountTypeErrorValues);
            Assert.IsTrue(accountTypeErrorValues.Single() == $"The {nameof(bankAccountCreation.CustomerId)} field is required.");
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
                Balance = 0,
                DebitLimit = 0,
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

            _bankAccountNumberBuilderMock.Setup(anf => anf.GenerateBankAccountNumber(null)).Returns(bankAccountNumber);

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByCustomer(bankAccountCreation);

            // Assert
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRole.Customer.ToString()), Times.Once);

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
            Assert.AreEqual(expectedBankAccount.Balance, bankAccountDto.Balance);
            Assert.AreEqual(expectedBankAccount.DebitLimit, bankAccountDto.DebitLimit);
            Assert.AreNotEqual(DateTime.MinValue, bankAccountDto.OpenedDate);
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
            Assert.AreEqual(expectedBankAccount.Balance, bankAccountFromDb.Balance);
            Assert.AreEqual(expectedBankAccount.DebitLimit, bankAccountFromDb.DebitLimit);
            Assert.AreNotEqual(DateTime.MinValue, bankAccountFromDb.OpenedDate);
            Assert.AreEqual(bankAccountFromDb.CreatedById, bankAccountFromDb.Customer.Id);
            Assert.AreEqual(bankAccountDto.Customer.Id, bankAccountDto.Customer.ApplicationUser.Id);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Name, bankAccountFromDb.Customer.ApplicationUser.Name);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Surname, bankAccountFromDb.Customer.ApplicationUser.Surname);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Email, bankAccountFromDb.Customer.ApplicationUser.Email);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.PhoneNumber, bankAccountFromDb.Customer.ApplicationUser.PhoneNumber);
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByCustomer_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountWithCustomerCreationByCustomerDto();
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.Register), $"The {nameof(bankAccountCreation.Register)} field is required.");
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.BankAccount), $"The {nameof(bankAccountCreation.BankAccount)} field is required.");

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByCustomer(bankAccountCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey(nameof(bankAccountCreation.Register)));
            Assert.IsTrue(error.ContainsKey(nameof(bankAccountCreation.BankAccount)));

            var registerErrorValues = error[nameof(bankAccountCreation.Register)] as string[];
            Assert.IsNotNull(registerErrorValues);
            Assert.IsTrue(registerErrorValues.Single() == $"The {nameof(bankAccountCreation.Register)} field is required.");

            var bankAccountErrorValues = error[nameof(bankAccountCreation.BankAccount)] as string[];
            Assert.IsNotNull(bankAccountErrorValues);
            Assert.IsTrue(bankAccountErrorValues.Single() == $"The {nameof(bankAccountCreation.BankAccount)} field is required.");
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
                Balance = 0,
                DebitLimit = 0,
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

            _sut.ControllerContext = context;

            _userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(() => true);

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser>(user =>
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                });

            _bankAccountNumberBuilderMock.Setup(anf => anf.GenerateBankAccountNumber(It.IsAny<int>())).Returns(bankAccountNumber);

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByWorker(bankAccountCreation);

            // Assert
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRole.Customer.ToString()), Times.Once);

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
            Assert.AreEqual(expectedBankAccount.Balance, bankAccountDto.Balance);
            Assert.AreEqual(expectedBankAccount.DebitLimit, bankAccountDto.DebitLimit);
            Assert.AreNotEqual(DateTime.MinValue, bankAccountDto.OpenedDate);
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
            Assert.AreEqual(expectedBankAccount.Balance, bankAccountFromDb.Balance);
            Assert.AreEqual(expectedBankAccount.DebitLimit, bankAccountFromDb.DebitLimit);
            Assert.AreNotEqual(DateTime.MinValue, bankAccountFromDb.OpenedDate);
            Assert.AreEqual(currentUser.Id, bankAccountFromDb.CreatedById);
            Assert.AreEqual(bankAccountFromDb.Customer.Id, bankAccountFromDb.Customer.ApplicationUser.Id);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Name, bankAccountFromDb.Customer.ApplicationUser.Name);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Surname, bankAccountFromDb.Customer.ApplicationUser.Surname);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.Email, bankAccountFromDb.Customer.ApplicationUser.Email);
            Assert.AreEqual(expectedBankAccount.Customer.ApplicationUser.PhoneNumber, bankAccountFromDb.Customer.ApplicationUser.PhoneNumber);
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByWorker_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountWithCustomerCreationByWorkerDto();
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.Register), $"The {nameof(bankAccountCreation.Register)} field is required.");
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.BankAccount), $"The {nameof(bankAccountCreation.BankAccount)} field is required.");

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByWorker(bankAccountCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey(nameof(bankAccountCreation.Register)));
            Assert.IsTrue(error.ContainsKey(nameof(bankAccountCreation.BankAccount)));

            var registerErrorValues = error[nameof(bankAccountCreation.Register)] as string[];
            Assert.IsNotNull(registerErrorValues);
            Assert.IsTrue(registerErrorValues.Single() == $"The {nameof(bankAccountCreation.Register)} field is required.");

            var bankAccountErrorValues = error[nameof(bankAccountCreation.BankAccount)] as string[];
            Assert.IsNotNull(bankAccountErrorValues);
            Assert.IsTrue(bankAccountErrorValues.Single() == $"The {nameof(bankAccountCreation.BankAccount)} field is required.");
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByWorker_Should_ReturnBadRequest_When_UserFromClaimsNotExist()
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

            var currentUser = new ApplicationUser { Id = 999 };
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

            _sut.ControllerContext = context;

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByWorker(bankAccountCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey(nameof(currentUser)));

            var currentUserErrorValues = error[nameof(currentUser)] as string[];
            Assert.IsNotNull(currentUserErrorValues);
            Assert.IsTrue(currentUserErrorValues.Single() == $"User with id {currentUser.Id} found in claims doesn't exist in database.");
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByWorker_Should_ReturnBadRequest_When_TellerIsNotAssignedToBranch()
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

            var currentUser = new ApplicationUser { Id = 3 };
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

            _sut.ControllerContext = context;

            _userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), UserRole.Teller.ToString()))
                .ReturnsAsync(() => true);

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByWorker(bankAccountCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey(nameof(currentUser.Teller)));

            var currentUserErrorValues = error[nameof(currentUser.Teller)] as string[];
            Assert.IsNotNull(currentUserErrorValues);
            Assert.IsTrue(currentUserErrorValues.Single() == $"Worker with id {currentUser.Id} is currently not assigned to any branch.");
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByWorker_Should_ReturnBadRequest_When_ManagerIsNotAssignedToBranch()
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

            var currentUser = new ApplicationUser { Id = 4 };
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

            _sut.ControllerContext = context;

            _userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), UserRole.Manager.ToString()))
                .ReturnsAsync(() => true);

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByWorker(bankAccountCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey(nameof(currentUser.Manager)));

            var currentUserErrorValues = error[nameof(currentUser.Manager)] as string[];
            Assert.IsNotNull(currentUserErrorValues);
            Assert.IsTrue(currentUserErrorValues.Single() == $"Worker with id {currentUser.Id} is currently not assigned to any branch.");
        }
    }
}