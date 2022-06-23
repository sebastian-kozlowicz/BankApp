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
        [Route("{cardId}", Name = "GetPaymentCard")]
        public async Task<ActionResult<PaymentCardDto>> GetPaymentCardAsync(int cardId)
        {
            var paymentCard = await _paymentCardService.GetPaymentCardAsync(cardId);

            if (paymentCard == null)
                return NotFound();

            return Ok(_mapper.Map<PaymentCard, PaymentCardDto>(paymentCard));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentCardDto>>> GetPaymentCardsAsync()
        {
            var paymentCards = await _paymentCardService.GetPaymentCardsAsync();

            if (!paymentCards.Any())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<PaymentCard>, IEnumerable<PaymentCardDto>>(paymentCards));
        }

        [HttpPost]
        public async Task<ActionResult<PaymentCardDto>> CreatePaymentCardAsync([FromBody] CardCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var paymentCardAsync = await _paymentCardService.CreatePaymentCardAsync(model);

            return Ok(_mapper.Map<PaymentCard, PaymentCardDto>(paymentCardAsync));
        }
    }
}