using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Dtos.Card;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentCardsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPaymentCardService _paymentCardService;

        public PaymentCardsController(IPaymentCardService paymentCardService, IMapper mapper)
        {
            _paymentCardService = paymentCardService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentCardDto>>> GetCardsAsync()
        {
            var cards = await _paymentCardService.GetCardsAsync();

            if (!cards.Any())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<PaymentCard>, IEnumerable<PaymentCardDto>>(cards));
        }

        [HttpPost]
        public async Task<ActionResult<PaymentCardDto>> CreateCardAsync([FromBody] CardCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var card = await _paymentCardService.CreateCardAsync(model);

            return Ok(_mapper.Map<PaymentCard, PaymentCardDto>(card));
        }
    }
}