using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Configuration;
using BankApp.Controllers;
using BankApp.Dtos.Address;
using BankApp.Dtos.ApplicationUser;
using BankApp.Dtos.Auth;
using BankApp.Dtos.BankAccount;
using BankApp.Dtos.BankAccount.WithCustomerCreation;
using BankApp.Dtos.Customer;
using BankApp.Enumerators;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Mapping;
using BankApp.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BankAccountCreationDto = BankApp.Dtos.BankAccount.BankAccountCreationDto;

namespace BankApp.UnitTests.Controllers
{
    [TestClass]
    public class BankAccountsControllerTests
    {
        private readonly BankAccount _bankAccount = new()
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

        private readonly BankAccountDto _bankAccountDto = new()
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

        private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        private Mock<IBankAccountService> _bankAccountServiceMock;
        private BankAccountsController _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _bankAccountServiceMock = new Mock<IBankAccountService>();
            _sut = new BankAccountsController(_mapper, _bankAccountServiceMock.Object);
        }

        [TestMethod]
        public async Task GetBankAccountAsync_Should_ReturnBankAccountDto_When_BankAccountIsFound()
        {
            // Arrange
            _bankAccountServiceMock.Setup(s => s.GetBankAccountAsync(It.IsAny<int>())).ReturnsAsync(_bankAccount);

            // Act
            var result = await _sut.GetBankAccountAsync(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            okResult.Value.Should().BeEquivalentTo(_bankAccountDto);
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetBankAccountAsync_Should_ReturnNotFound_When_BankAccountNotFound()
        {
            //Arrange
            _bankAccountServiceMock.Setup(s => s.GetBankAccountAsync(It.IsAny<int>())).ReturnsAsync((BankAccount)null);

            // Act
            var result = await _sut.GetBankAccountAsync(999);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();

            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task GetBankAccountsForUserAsync_Should_ReturnBankAccountDtoList_When_BankAccountsAreFound()
        {
            //Arrange
            _bankAccountServiceMock.Setup(s => s.GetBankAccountsForUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<BankAccount> { _bankAccount });

            // Act
            var result = await _sut.GetBankAccountsForUserAsync(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            okResult.Value.Should().BeEquivalentTo(new List<BankAccountDto> { _bankAccountDto });
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetBankAccountsForUserAsync_Should_ReturnNotFound_When_BankAccountsNotFound()
        {
            //Arrange
            _bankAccountServiceMock.Setup(s => s.GetBankAccountsForUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<BankAccount>());

            // Act
            var result = await _sut.GetBankAccountsForUserAsync(999);

            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();

            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task
            CreateBankAccountAsync_Should_CreateBankAccount_And_ReturnBankAccountDto_When_ModelStateIsValid()
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
                CheckDigits = "61",
                NationalBankCode = "1080",
                BranchCode = "000",
                NationalCheckDigit = 1,
                AccountNumber = 0,
                AccountNumberText = "0000000000000000",
                Iban = "PL61108000010000000000000000",
                IbanSeparated = "PL 61 1080 0001 0000 0000 0000 0000"
            };

            var bankAccount = new BankAccount
            {
                AccountType = (AccountType)bankAccountCreation.AccountType,
                Currency = (Currency)bankAccountCreation.Currency,
                CountryCode = bankAccountNumber.CountryCode,
                CheckDigits = bankAccountNumber.CheckDigits,
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

            var expectedBankAccountDto = new BankAccountDto
            {
                AccountType = (AccountType)bankAccountCreation.AccountType,
                Currency = (Currency)bankAccountCreation.Currency,
                CountryCode = bankAccountNumber.CountryCode,
                CheckDigits = bankAccountNumber.CheckDigits,
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

            _bankAccountServiceMock.Setup(s => s.CreateBankAccountAsync(It.IsAny<BankAccountCreationDto>()))
                .ReturnsAsync(bankAccount);

            // Act
            var result = await _sut.CreateBankAccountAsync(bankAccountCreation);

            // Assert
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            createdAtRouteResult.Should().NotBeNull();

            createdAtRouteResult.Value.Should().BeEquivalentTo(expectedBankAccountDto);
            createdAtRouteResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task CreateBankAccountAsync_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountCreationDto();
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.AccountType),
                $"The {nameof(bankAccountCreation.AccountType)} field is required.");
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.Currency),
                $"The {nameof(bankAccountCreation.Currency)} field is required.");
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.CustomerId),
                $"The {nameof(bankAccountCreation.CustomerId)} field is required.");

            var expectedResult = new SerializableError
            {
                {
                    nameof(bankAccountCreation.AccountType),
                    new[] { $"The {nameof(bankAccountCreation.AccountType)} field is required." }
                },
                {
                    nameof(bankAccountCreation.Currency),
                    new[] { $"The {nameof(bankAccountCreation.Currency)} field is required." }
                },
                {
                    nameof(bankAccountCreation.CustomerId),
                    new[] { $"The {nameof(bankAccountCreation.CustomerId)} field is required." }
                }
            };

            // Act
            var result = await _sut.CreateBankAccountAsync(bankAccountCreation);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task
            CreateBankAccountWithCustomerByCustomerAsync_Should_CreateBankAccountWithCustomer_And_ReturnBankAccountDto_When_ModelStateIsValid()
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
                CheckDigits = "61",
                NationalBankCode = "1080",
                BranchCode = "000",
                NationalCheckDigit = 1,
                AccountNumber = 0,
                AccountNumberText = "0000000000000000",
                Iban = "PL61108000010000000000000000",
                IbanSeparated = "PL 61 1080 0001 0000 0000 0000 0000"
            };

            var bankAccount = new BankAccount
            {
                AccountType = (AccountType)bankAccountCreation.BankAccount.AccountType,
                Currency = (Currency)bankAccountCreation.BankAccount.Currency,
                CountryCode = bankAccountNumber.CountryCode,
                CheckDigits = bankAccountNumber.CheckDigits,
                NationalBankCode = bankAccountNumber.NationalBankCode,
                BranchCode = bankAccountNumber.BranchCode,
                NationalCheckDigit = bankAccountNumber.NationalCheckDigit,
                AccountNumber = bankAccountNumber.AccountNumber,
                AccountNumberText = bankAccountNumber.AccountNumberText,
                Iban = bankAccountNumber.Iban,
                IbanSeparated = bankAccountNumber.IbanSeparated,
                Balance = 0,
                DebitLimit = 0,
                Customer = new Customer
                {
                    ApplicationUser = new ApplicationUser
                    {
                        Name = bankAccountCreation.Register.User.Name,
                        Surname = bankAccountCreation.Register.User.Surname,
                        Email = bankAccountCreation.Register.User.Email,
                        PhoneNumber = bankAccountCreation.Register.User.PhoneNumber
                    }
                }
            };

            var expectedBankAccountDto = new BankAccountDto
            {
                AccountType = (AccountType)bankAccountCreation.BankAccount.AccountType,
                Currency = (Currency)bankAccountCreation.BankAccount.Currency,
                CountryCode = bankAccountNumber.CountryCode,
                CheckDigits = bankAccountNumber.CheckDigits,
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

            _bankAccountServiceMock.Setup(s =>
                    s.CreateBankAccountWithCustomerByCustomerAsync(
                        It.IsAny<BankAccountWithCustomerCreationByCustomerDto>()))
                .ReturnsAsync(bankAccount);

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByCustomerAsync(bankAccountCreation);

            // Assert
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            createdAtRouteResult.Should().NotBeNull();

            createdAtRouteResult.Value.Should().BeEquivalentTo(expectedBankAccountDto,
                options => options.Excluding(ba => ba.Customer.ApplicationUser.ConcurrencyStamp));
            createdAtRouteResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task
            CreateBankAccountWithCustomerByCustomerAsync_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountWithCustomerCreationByCustomerDto();
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.Register),
                $"The {nameof(bankAccountCreation.Register)} field is required.");
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.BankAccount),
                $"The {nameof(bankAccountCreation.BankAccount)} field is required.");

            var expectedResult = new SerializableError
            {
                {
                    nameof(bankAccountCreation.Register),
                    new[] { $"The {nameof(bankAccountCreation.Register)} field is required." }
                },
                {
                    nameof(bankAccountCreation.BankAccount),
                    new[] { $"The {nameof(bankAccountCreation.BankAccount)} field is required." }
                }
            };

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByCustomerAsync(bankAccountCreation);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task
            CreateBankAccountWithCustomerByWorkerAsync_Should_CreateBankAccountWithCustomer_And_ReturnBankAccountDto_When_ModelStateIsValid()
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
                CheckDigits = "61",
                NationalBankCode = "1080",
                BranchCode = "000",
                NationalCheckDigit = 1,
                AccountNumber = 0,
                AccountNumberText = "0000000000000000",
                Iban = "PL61108000010000000000000000",
                IbanSeparated = "PL 61 1080 0001 0000 0000 0000 0000"
            };

            var bankAccount = new BankAccount
            {
                AccountType = (AccountType)bankAccountCreation.BankAccount.AccountType,
                Currency = (Currency)bankAccountCreation.BankAccount.Currency,
                CountryCode = bankAccountNumber.CountryCode,
                CheckDigits = bankAccountNumber.CheckDigits,
                NationalBankCode = bankAccountNumber.NationalBankCode,
                BranchCode = bankAccountNumber.BranchCode,
                NationalCheckDigit = bankAccountNumber.NationalCheckDigit,
                AccountNumber = bankAccountNumber.AccountNumber,
                AccountNumberText = bankAccountNumber.AccountNumberText,
                Iban = bankAccountNumber.Iban,
                IbanSeparated = bankAccountNumber.IbanSeparated,
                Balance = 0,
                DebitLimit = 0,
                Customer = new Customer
                {
                    ApplicationUser = new ApplicationUser
                    {
                        Name = bankAccountCreation.Register.User.Name,
                        Surname = bankAccountCreation.Register.User.Surname,
                        Email = bankAccountCreation.Register.User.Email,
                        PhoneNumber = bankAccountCreation.Register.User.PhoneNumber
                    }
                }
            };

            var expectedBankAccountDto = new BankAccountDto
            {
                AccountType = (AccountType)bankAccountCreation.BankAccount.AccountType,
                Currency = (Currency)bankAccountCreation.BankAccount.Currency,
                CountryCode = bankAccountNumber.CountryCode,
                CheckDigits = bankAccountNumber.CheckDigits,
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

            _bankAccountServiceMock
                .Setup(s => s.CreateBankAccountWithCustomerByWorkerAsync(
                    It.IsAny<BankAccountWithCustomerCreationByWorkerDto>(), It.IsAny<int>()))
                .ReturnsAsync(bankAccount);

            var currentUser = new ApplicationUser { Id = 1 };
            var claims = new List<Claim> { new(CustomClaimTypes.UserId, currentUser.Id.ToString()) };
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
            var result = await _sut.CreateBankAccountWithCustomerByWorkerAsync(bankAccountCreation);

            // Assert
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            createdAtRouteResult.Should().NotBeNull();

            createdAtRouteResult.Value.Should().BeEquivalentTo(expectedBankAccountDto,
                options => options.Excluding(ba => ba.Customer.ApplicationUser.ConcurrencyStamp));
            createdAtRouteResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task CreateBankAccountWithCustomerByWorkerAsync_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var bankAccountCreation = new BankAccountWithCustomerCreationByWorkerDto();
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.Register),
                $"The {nameof(bankAccountCreation.Register)} field is required.");
            _sut.ModelState.AddModelError(nameof(bankAccountCreation.BankAccount),
                $"The {nameof(bankAccountCreation.BankAccount)} field is required.");

            var expectedResult = new SerializableError
            {
                {
                    nameof(bankAccountCreation.Register),
                    new[] { $"The {nameof(bankAccountCreation.Register)} field is required." }
                },
                {
                    nameof(bankAccountCreation.BankAccount),
                    new[] { $"The {nameof(bankAccountCreation.BankAccount)} field is required." }
                }
            };

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByWorkerAsync(bankAccountCreation);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().BeEquivalentTo(expectedResult);
        }
    }
}