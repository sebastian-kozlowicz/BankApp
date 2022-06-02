using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Address;
using BankApp.Dtos.ApplicationUser;
using BankApp.Dtos.Auth;
using BankApp.Dtos.BankAccount.WithCustomerCreation;
using BankApp.Enumerators;
using BankApp.Helpers.Services;
using BankApp.Interfaces.Helpers.Builders.Number;
using BankApp.Mapping;
using BankApp.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BankAccountCreationDto = BankApp.Dtos.BankAccount.BankAccountCreationDto;

namespace BankApp.UnitTests.Helpers.Services
{
    [TestClass]
    public class BankAccountServiceTests
    {
        private readonly ApplicationUser _customerApplicationUser = new()
        {
            Id = 1,
            Customer = new Customer
            {
                Id = 1
            }
        };

        private readonly BankAccount _firstBankAccount = new()
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

        private readonly BankAccount _secondBankAccount = new()
        {
            Id = 2,
            AccountType = AccountType.Savings,
            Currency = Currency.Pln,
            CountryCode = "PL",
            CheckDigits = "27",
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

        private Mock<IBankAccountNumberBuilder> _bankAccountNumberBuilderMock;
        private ApplicationDbContext _context;

        private BankAccountService _sut;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<IUserStore<ApplicationUser>> _userStoreMock;

        private IEnumerable<BankAccount> BankAccounts =>
            new List<BankAccount> { _firstBankAccount, _secondBankAccount };

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
            context.Users.Add(_customerApplicationUser);
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
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(_userStoreMock.Object, null, null, null, null,
                null, null, null, null);
            _bankAccountNumberBuilderMock = new Mock<IBankAccountNumberBuilder>();
            _sut = new BankAccountService(_context, _mapper, _userManagerMock.Object,
                _bankAccountNumberBuilderMock.Object);
        }

        [TestMethod]
        public async Task GetBankAccountAsync_Should_ReturnBankAccount_When_BankAccountIsFound()
        {
            var result = await _sut.GetBankAccountAsync(1);

            result.Should().Be(_firstBankAccount);
        }

        [TestMethod]
        public async Task GetBankAccountAsync_Should_ReturnNull_When_BankAccountNotFound()
        {
            var result = await _sut.GetBankAccountAsync(999);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetBankAccountsForUserAsync_Should_ReturnBankAccountList_When_BankAccountsAreFound()
        {
            var result = await _sut.GetBankAccountsForUserAsync(1);

            result.Should().NotBeNullOrEmpty();
            result.ToList()[0].Should().Be(_firstBankAccount);
            result.ToList()[1].Should().Be(_secondBankAccount);
        }

        [TestMethod]
        public async Task GetBankAccountsForUserAsync_Should_ReturnEmptyBankAccountList_When_BankAccountsNotFound()
        {
            var result = await _sut.GetBankAccountsForUserAsync(999);

            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task CreateBankAccountAsync_Should_CreateBankAccount_And_ReturnBankAccount()
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

            var expectedBankAccount = new BankAccount
            {
                Id = 3,
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
                Customer = _customerApplicationUser.Customer,
                CreatedById = (int)bankAccountCreation.CustomerId,
                CreatedBy = _customerApplicationUser
            };

            _bankAccountNumberBuilderMock.Setup(anf => anf.GenerateBankAccountNumber(null)).Returns(bankAccountNumber);

            // Act
            var result = await _sut.CreateBankAccountAsync(bankAccountCreation);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedBankAccount,
                options => options.Excluding(b => b.OpenedDate));
            result.OpenedDate.Should().NotBe(DateTime.MinValue);

            var bankAccountFromDb = _context.BankAccounts.SingleOrDefault(ba => ba.Id == result.Id);
            bankAccountFromDb.Should().NotBeNull();
            bankAccountFromDb.Should().BeEquivalentTo(expectedBankAccount,
                options => options.Excluding(b => b.OpenedDate));
            bankAccountFromDb.OpenedDate.Should().NotBe(DateTime.MinValue);
        }

        [TestMethod]
        public async Task
            CreateBankAccountWithCustomerByCustomerAsync_Should_CreateBankAccountWithCustomer_And_ReturnBankAccount()
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

            var expectedCustomer = new Customer
            {
                Id = 5,
                ApplicationUser = new ApplicationUser
                {
                    Id = 5,
                    Name = bankAccountCreation.Register.User.Name,
                    Surname = bankAccountCreation.Register.User.Surname,
                    Email = bankAccountCreation.Register.User.Email,
                    PhoneNumber = bankAccountCreation.Register.User.PhoneNumber,
                    UserAddress = new UserAddress
                    {
                        Id = 5,
                        Country = bankAccountCreation.Register.Address.Country,
                        City = bankAccountCreation.Register.Address.City,
                        Street = bankAccountCreation.Register.Address.Street,
                        HouseNumber = bankAccountCreation.Register.Address.HouseNumber,
                        ApartmentNumber = bankAccountCreation.Register.Address.ApartmentNumber,
                        PostalCode = bankAccountCreation.Register.Address.PostalCode
                    }
                }
            };

            var expectedBankAccount = new BankAccount
            {
                Id = 3,
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
                CustomerId = 5,
                CreatedById = 5
            };

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser, string>((user, password) =>
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                });

            _bankAccountNumberBuilderMock.Setup(anf => anf.GenerateBankAccountNumber(null)).Returns(bankAccountNumber);

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByCustomerAsync(bankAccountCreation);

            // Assert
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRole.Customer.ToString()),
                Times.Once);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedBankAccount,
                options => options.Excluding(b => b.OpenedDate).Excluding(b => b.Customer).Excluding(b => b.CreatedBy));
            result.OpenedDate.Should().NotBe(DateTime.MinValue);
            result.Customer.Id.Should().Be(expectedCustomer.Id);
            result.Customer.ApplicationUser.Id.Should().Be(expectedCustomer.Id);
            result.Customer.ApplicationUser.Name.Should().Be(expectedCustomer.ApplicationUser.Name);
            result.Customer.ApplicationUser.Surname.Should().Be(expectedCustomer.ApplicationUser.Surname);
            result.Customer.ApplicationUser.Email.Should().Be(expectedCustomer.ApplicationUser.Email);
            result.Customer.ApplicationUser.PhoneNumber.Should().Be(expectedCustomer.ApplicationUser.PhoneNumber);
            result.Customer.ApplicationUser.UserAddress.Id.Should().Be(expectedCustomer.ApplicationUser.UserAddress.Id);
            result.Customer.ApplicationUser.UserAddress.Country.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Country);
            result.Customer.ApplicationUser.UserAddress.City.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.City);
            result.Customer.ApplicationUser.UserAddress.Street.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Street);
            result.Customer.ApplicationUser.UserAddress.HouseNumber.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.HouseNumber);
            result.Customer.ApplicationUser.UserAddress.ApartmentNumber.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.ApartmentNumber);
            result.Customer.ApplicationUser.UserAddress.PostalCode.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.PostalCode);

            var bankAccountFromDb = _context.BankAccounts.SingleOrDefault(ba => ba.Id == result.Id);
            bankAccountFromDb.Should().NotBeNull();
            bankAccountFromDb.Should().BeEquivalentTo(expectedBankAccount,
                options => options.Excluding(b => b.OpenedDate).Excluding(b => b.Customer).Excluding(b => b.CreatedBy));
            bankAccountFromDb.OpenedDate.Should().NotBe(DateTime.MinValue);
            bankAccountFromDb.Customer.Id.Should().Be(expectedCustomer.Id);
            bankAccountFromDb.Customer.ApplicationUser.Id.Should().Be(expectedCustomer.Id);
            bankAccountFromDb.Customer.ApplicationUser.Name.Should().Be(expectedCustomer.ApplicationUser.Name);
            bankAccountFromDb.Customer.ApplicationUser.Surname.Should().Be(expectedCustomer.ApplicationUser.Surname);
            bankAccountFromDb.Customer.ApplicationUser.Email.Should().Be(expectedCustomer.ApplicationUser.Email);
            bankAccountFromDb.Customer.ApplicationUser.PhoneNumber.Should()
                .Be(expectedCustomer.ApplicationUser.PhoneNumber);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.Id.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Id);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.Country.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Country);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.City.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.City);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.Street.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Street);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.HouseNumber.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.HouseNumber);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.ApartmentNumber.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.ApartmentNumber);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.PostalCode.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.PostalCode);
        }

        [TestMethod]
        public async Task
            CreateBankAccountWithCustomerByWorkerAsync_Should_CreateBankAccountWithCustomer_And_ReturnBankAccount()
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

            const int currentUserId = 2;

            var expectedCustomer = new Customer
            {
                Id = 5,
                ApplicationUser = new ApplicationUser
                {
                    Id = 5,
                    Name = bankAccountCreation.Register.User.Name,
                    Surname = bankAccountCreation.Register.User.Surname,
                    Email = bankAccountCreation.Register.User.Email,
                    PhoneNumber = bankAccountCreation.Register.User.PhoneNumber,
                    UserAddress = new UserAddress
                    {
                        Id = 5,
                        Country = bankAccountCreation.Register.Address.Country,
                        City = bankAccountCreation.Register.Address.City,
                        Street = bankAccountCreation.Register.Address.Street,
                        HouseNumber = bankAccountCreation.Register.Address.HouseNumber,
                        ApartmentNumber = bankAccountCreation.Register.Address.ApartmentNumber,
                        PostalCode = bankAccountCreation.Register.Address.PostalCode
                    }
                }
            };

            var expectedBankAccount = new BankAccount
            {
                Id = 3,
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
                CustomerId = 5,
                CreatedById = currentUserId
            };

            _userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(() => true);

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success).Callback<ApplicationUser>(user =>
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                });

            _bankAccountNumberBuilderMock.Setup(anf => anf.GenerateBankAccountNumber(It.IsAny<int>()))
                .Returns(bankAccountNumber);

            // Act
            var result = await _sut.CreateBankAccountWithCustomerByWorkerAsync(bankAccountCreation, currentUserId);

            // Assert
            _userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRole.Customer.ToString()),
                Times.Once);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedBankAccount,
                options => options.Excluding(b => b.OpenedDate).Excluding(b => b.Customer).Excluding(b => b.CreatedBy));
            result.OpenedDate.Should().NotBe(DateTime.MinValue);
            result.Customer.Id.Should().Be(expectedCustomer.Id);
            result.Customer.ApplicationUser.Id.Should().Be(expectedCustomer.Id);
            result.Customer.ApplicationUser.Name.Should().Be(expectedCustomer.ApplicationUser.Name);
            result.Customer.ApplicationUser.Surname.Should().Be(expectedCustomer.ApplicationUser.Surname);
            result.Customer.ApplicationUser.Email.Should().Be(expectedCustomer.ApplicationUser.Email);
            result.Customer.ApplicationUser.PhoneNumber.Should().Be(expectedCustomer.ApplicationUser.PhoneNumber);
            result.Customer.ApplicationUser.CreatedById.Should().Be(currentUserId);
            result.Customer.ApplicationUser.UserAddress.Id.Should().Be(expectedCustomer.ApplicationUser.UserAddress.Id);
            result.Customer.ApplicationUser.UserAddress.Country.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Country);
            result.Customer.ApplicationUser.UserAddress.City.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.City);
            result.Customer.ApplicationUser.UserAddress.Street.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Street);
            result.Customer.ApplicationUser.UserAddress.HouseNumber.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.HouseNumber);
            result.Customer.ApplicationUser.UserAddress.ApartmentNumber.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.ApartmentNumber);
            result.Customer.ApplicationUser.UserAddress.PostalCode.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.PostalCode);

            var bankAccountFromDb = _context.BankAccounts.SingleOrDefault(ba => ba.Id == result.Id);
            bankAccountFromDb.Should().NotBeNull();
            bankAccountFromDb.Should().BeEquivalentTo(expectedBankAccount,
                options => options.Excluding(b => b.OpenedDate).Excluding(b => b.Customer).Excluding(b => b.CreatedBy));
            bankAccountFromDb.OpenedDate.Should().NotBe(DateTime.MinValue);
            bankAccountFromDb.Customer.Id.Should().Be(expectedCustomer.Id);
            bankAccountFromDb.Customer.ApplicationUser.Id.Should().Be(expectedCustomer.Id);
            bankAccountFromDb.Customer.ApplicationUser.Name.Should().Be(expectedCustomer.ApplicationUser.Name);
            bankAccountFromDb.Customer.ApplicationUser.Surname.Should().Be(expectedCustomer.ApplicationUser.Surname);
            bankAccountFromDb.Customer.ApplicationUser.Email.Should().Be(expectedCustomer.ApplicationUser.Email);
            bankAccountFromDb.Customer.ApplicationUser.PhoneNumber.Should()
                .Be(expectedCustomer.ApplicationUser.PhoneNumber);
            bankAccountFromDb.Customer.ApplicationUser.CreatedById.Should().Be(currentUserId);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.Id.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Id);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.Country.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Country);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.City.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.City);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.Street.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.Street);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.HouseNumber.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.HouseNumber);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.ApartmentNumber.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.ApartmentNumber);
            bankAccountFromDb.Customer.ApplicationUser.UserAddress.PostalCode.Should()
                .Be(expectedCustomer.ApplicationUser.UserAddress.PostalCode);
        }
    }
}