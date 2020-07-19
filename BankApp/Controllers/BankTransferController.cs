using BankApp.Data;
using BankApp.Dtos.BankTransfer;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankTransferController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BankTransferController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult CreateBankTransfer([FromBody] BankTransferCreationDto bankTransferCreationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bankAccount = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankTransferCreationDto.RequesterBankAccountId);

            if (bankAccount == null)
                return NotFound();

            var targetBankAccount = _context.BankAccounts.FirstOrDefault(ba => ba.Iban == bankTransferCreationDto.ReceiverIban);

            bankAccount.Balance -= (decimal)bankTransferCreationDto.Value;
            targetBankAccount.Balance += (decimal)bankTransferCreationDto.Value;

            _context.SaveChanges();

            return Ok();
        }
    }
}
