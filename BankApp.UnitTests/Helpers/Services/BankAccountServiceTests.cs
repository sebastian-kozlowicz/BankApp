﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
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

namespace BankApp.UnitTests.Helpers.Services
{
    [TestClass]
    public class BankAccountServiceTests
    {
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
    }
}