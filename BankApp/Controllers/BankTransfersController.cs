using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IBankTransferService _bankTransferService;

        public BankTransfersController(IBankTransferService bankTransferService)
        {
            _bankTransferService = bankTransferService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateBankTransferAsync([FromBody] BankTransferCreationDto bankTransferCreationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _bankTransferService.CreateBankTransferAsync(bankTransferCreationDto);
            return Ok();
        }
    }
}