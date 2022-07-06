using System;
using System.Linq;
using BankApp.Data;
using BankApp.Dtos.BankTransfer;
using BankApp.Helpers.Handlers;
using BankApp.Helpers.Services;
using BankApp.Interfaces.Helpers.Handlers;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankTransfersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransferHandler<ExternalTransferHandler> _externalTransferHandler;
        private readonly ITransferHandler<InternalTransferHandler> _internalTransferHandler;

        public BankTransfersController(ApplicationDbContext context,
            ITransferHandler<InternalTransferHandler> internalTransferHandler,
            ITransferHandler<ExternalTransferHandler> externalTransferHandler)
        {
            _context = context;
            _internalTransferHandler = internalTransferHandler;
            _externalTransferHandler = externalTransferHandler;
        }

        [HttpPost]
        public ActionResult CreateBankTransfer([FromBody] BankTransferCreationDto bankTransferCreationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var requesterBankAccount =
                _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankTransferCreationDto.RequesterBankAccountId);

            if (requesterBankAccount == null)
                return NotFound();

            if (requesterBankAccount.Balance - bankTransferCreationDto.Value < requesterBankAccount.DebitLimit * -1)
            {
                ModelState.AddModelError(nameof(requesterBankAccount.DebitLimit),
                    "Not sufficient founds. Debit limit is exceeded.");
                return BadRequest(ModelState);
            }

            var targetBankAccount =
                _context.BankAccounts.FirstOrDefault(ba => ba.Iban == bankTransferCreationDto.ReceiverIban);

            if (targetBankAccount == null)
                _externalTransferHandler.CreateBankTransferAsync(requesterBankAccount,
                    new BankAccount {Iban = bankTransferCreationDto.ReceiverIban},
                    (decimal) bankTransferCreationDto.Value);
            else
                try
                {
                    _internalTransferHandler.CreateBankTransferAsync(requesterBankAccount, targetBankAccount,
                        (decimal) bankTransferCreationDto.Value);
                }
                catch (ArgumentException exception)
                {
                    ModelState.AddModelError(exception.ParamName, exception.Message);
                    return BadRequest(ModelState);
                }

            return Ok();
        }
    }
}