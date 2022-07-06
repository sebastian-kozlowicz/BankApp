using System;
using System.Threading.Tasks;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Exceptions;
using BankApp.Interfaces.Helpers.Handlers;
using BankApp.Models;

namespace BankApp.Helpers.Handlers
{
    public class InternalTransferHandler : ITransferHandler<InternalTransferHandler>
    {
        private readonly ApplicationDbContext _context;

        public InternalTransferHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateBankTransferAsync(BankAccount bankAccount, BankAccount targetBankAccount, decimal value)
        {
            if (bankAccount.Currency != targetBankAccount.Currency)
                throw new ValidationException("Currency is different in target bank account",
                    nameof(targetBankAccount.Currency));

            bankAccount.Balance -= value;
            targetBankAccount.Balance += value;

            var bankTransfer = new BankTransfer
            {
                ReceiverIban = targetBankAccount.Iban,
                OrderDate = DateTime.UtcNow,
                BankTransferType = BankTransferType.Internal,
                BankAccountId = bankAccount.Id
            };

            await _context.BankTransfers.AddAsync(bankTransfer);
            await _context.SaveChangesAsync();
        }
    }
}