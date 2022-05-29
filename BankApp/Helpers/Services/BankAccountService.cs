using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.BankAccount.WithCustomerCreation;
using BankApp.Enumerators;
using BankApp.Exceptions;
using BankApp.Interfaces.Helpers.Builders.Number;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BankAccountCreationDto = BankApp.Dtos.BankAccount.BankAccountCreationDto;

namespace BankApp.Helpers.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountNumberBuilder _bankAccountNumberBuilder;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public BankAccountService(ApplicationDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager, IBankAccountNumberBuilder bankAccountNumberBuilder)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _bankAccountNumberBuilder = bankAccountNumberBuilder;
        }

        public async Task<BankAccount> GetBankAccountAsync(int bankAccountId)
        {
            return await _context.BankAccounts.SingleOrDefaultAsync(ba => ba.Id == bankAccountId);
        }

        public async Task<IEnumerable<BankAccount>> GetBankAccountsForUserAsync(int applicationUserId)
        {
            return await _context.BankAccounts.Where(ba => ba.CustomerId == applicationUserId).ToListAsync();
        }

        public async Task<BankAccount> CreateBankAccountAsync(BankAccountCreationDto model)
        {
            var generatedAccountNumber = _bankAccountNumberBuilder.GenerateBankAccountNumber();

            var bankAccount = new BankAccount
            {
                AccountType = (AccountType)model.AccountType,
                Currency = (Currency)model.Currency,
                CountryCode = generatedAccountNumber.CountryCode,
                CheckDigits = generatedAccountNumber.CheckDigits,
                NationalBankCode = generatedAccountNumber.NationalBankCode,
                BranchCode = generatedAccountNumber.BranchCode,
                NationalCheckDigit = generatedAccountNumber.NationalCheckDigit,
                AccountNumber = generatedAccountNumber.AccountNumber,
                AccountNumberText = generatedAccountNumber.AccountNumberText,
                Iban = generatedAccountNumber.Iban,
                IbanSeparated = generatedAccountNumber.IbanSeparated,
                OpenedDate = DateTime.UtcNow,
                CustomerId = (int)model.CustomerId,
                CreatedById = (int)model.CustomerId
            };

            await _context.BankAccounts.AddAsync(bankAccount);
            await _context.SaveChangesAsync();

            return bankAccount;
        }

        public async Task<BankAccount> CreateBankAccountWithCustomerByCustomerAsync(
            BankAccountWithCustomerCreationByCustomerDto model)
        {
            var generatedAccountNumber = _bankAccountNumberBuilder.GenerateBankAccountNumber();

            var user = _mapper.Map<ApplicationUser>(model.Register);
            user.Customer = new Customer { Id = user.Id };

            var bankAccount = new BankAccount
            {
                AccountType = (AccountType)model.BankAccount.AccountType,
                Currency = (Currency)model.BankAccount.Currency,
                CountryCode = generatedAccountNumber.CountryCode,
                CheckDigits = generatedAccountNumber.CheckDigits,
                NationalBankCode = generatedAccountNumber.NationalBankCode,
                BranchCode = generatedAccountNumber.BranchCode,
                NationalCheckDigit = generatedAccountNumber.NationalCheckDigit,
                AccountNumber = generatedAccountNumber.AccountNumber,
                AccountNumberText = generatedAccountNumber.AccountNumberText,
                Iban = generatedAccountNumber.Iban,
                IbanSeparated = generatedAccountNumber.IbanSeparated,
                OpenedDate = DateTime.UtcNow,
                CustomerId = user.Id,
                CreatedById = user.Id
            };

            user.Customer.BankAccounts = new List<BankAccount> { bankAccount };
            user.CreatedBankAccounts = new List<BankAccount> { bankAccount };

            var result = await _userManager.CreateAsync(user, model.Register.User.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
            else
                throw new Exception(JsonConvert.SerializeObject(result.Errors));

            return bankAccount;
        }

        public async Task<BankAccount> CreateBankAccountWithCustomerByWorkerAsync(
            BankAccountWithCustomerCreationByWorkerDto model, int currentUserId)
        {
            var currentUser = _context.Users.SingleOrDefault(u => u.Id == currentUserId);
            if (currentUser == null)
                throw new InvalidDataInDatabaseException(
                    $"User with id {currentUserId} found in claims doesn't exist in database.");

            int? workerBranchId = null;

            if (await _userManager.IsInRoleAsync(currentUser, UserRole.Teller.ToString()))
            {
                await _context.Tellers.Where(t => t.Id == currentUser.Id).LoadAsync();

                if (currentUser.Teller.WorkAtId == null)
                    throw new InvalidDataInDatabaseException(
                        $"Worker with id {currentUserId} is currently not assigned to any branch.");

                workerBranchId = currentUser.Teller.WorkAtId;
            }
            else if (await _userManager.IsInRoleAsync(currentUser, UserRole.Manager.ToString()))
            {
                await _context.Managers.Where(m => m.Id == currentUser.Id).LoadAsync();

                if (currentUser.Manager.WorkAtId == null)
                    throw new InvalidDataInDatabaseException(
                        $"Worker with id {currentUserId} is currently not assigned to any branch.");

                workerBranchId = currentUser.Manager.WorkAtId;
            }

            var generatedAccountNumber = _bankAccountNumberBuilder.GenerateBankAccountNumber(workerBranchId);

            var user = _mapper.Map<ApplicationUser>(model.Register);
            user.Customer = new Customer { Id = user.Id };
            user.CreatedById = currentUserId;

            var bankAccount = new BankAccount
            {
                AccountType = (AccountType)model.BankAccount.AccountType,
                Currency = (Currency)model.BankAccount.Currency,
                CountryCode = generatedAccountNumber.CountryCode,
                CheckDigits = generatedAccountNumber.CheckDigits,
                NationalBankCode = generatedAccountNumber.NationalBankCode,
                BranchCode = generatedAccountNumber.BranchCode,
                NationalCheckDigit = generatedAccountNumber.NationalCheckDigit,
                AccountNumber = generatedAccountNumber.AccountNumber,
                AccountNumberText = generatedAccountNumber.AccountNumberText,
                Iban = generatedAccountNumber.Iban,
                IbanSeparated = generatedAccountNumber.IbanSeparated,
                OpenedDate = DateTime.UtcNow,
                CustomerId = user.Id,
                CreatedById = currentUserId
            };

            user.Customer.BankAccounts = new List<BankAccount> { bankAccount };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
            else
                throw new Exception(JsonConvert.SerializeObject(result.Errors));

            return bankAccount;
        }
    }
}