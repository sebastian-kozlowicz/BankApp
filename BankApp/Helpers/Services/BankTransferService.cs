using System.Threading.Tasks;
using BankApp.Data;
using BankApp.Dtos.BankTransfer;
using BankApp.Exceptions;
using BankApp.Helpers.Handlers;
using BankApp.Interfaces.Helpers.Handlers;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Helpers.Services
{
    public class BankTransferService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransferHandler<ExternalTransferHandler> _externalTransferHandler;
        private readonly ITransferHandler<InternalTransferHandler> _internalTransferHandler;

        public BankTransferService(ApplicationDbContext context,
            ITransferHandler<InternalTransferHandler> internalTransferHandler,
            ITransferHandler<ExternalTransferHandler> externalTransferHandler)
        {
            _context = context;
            _internalTransferHandler = internalTransferHandler;
            _externalTransferHandler = externalTransferHandler;
        }

        public async Task<bool> CreateBankTransferAsync(BankTransferCreationDto bankTransferCreationDto)
        {
            var requesterBankAccount =
                await _context.BankAccounts.SingleOrDefaultAsync(ba =>
                    ba.Id == bankTransferCreationDto.RequesterBankAccountId);

            if (requesterBankAccount == null)
                throw new NotFoundException();

            if (requesterBankAccount.Balance - bankTransferCreationDto.Value < requesterBankAccount.DebitLimit * -1)
                throw new ValidationException("Not sufficient founds. Debit limit is exceeded");

            var targetBankAccount =
                await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.Iban == bankTransferCreationDto.ReceiverIban);

            if (targetBankAccount == null)
                _externalTransferHandler.CreateBankTransferAsync(requesterBankAccount,
                    new BankAccount { Iban = bankTransferCreationDto.ReceiverIban },
                    (decimal)bankTransferCreationDto.Value);
            else
                _internalTransferHandler.CreateBankTransferAsync(requesterBankAccount, targetBankAccount,
                    (decimal)bankTransferCreationDto.Value);

            return true;
        }
    }
}