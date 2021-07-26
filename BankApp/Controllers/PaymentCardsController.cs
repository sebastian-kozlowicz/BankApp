using AutoMapper;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Dtos.Card;
using BankApp.Enumerators;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using BankApp.Interfaces.Helpers.Factories;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentCardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPaymentCardNumberFactory _paymentCardNumberFactory;

        public PaymentCardsController(ApplicationDbContext context, IMapper mapper, IPaymentCardNumberFactory paymentCardNumberFactory)
        {
            _context = context;
            _mapper = mapper;
            _paymentCardNumberFactory = paymentCardNumberFactory;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PaymentCardDto>> GetCards()
        {
            var cards = _context.PaymentCards.ToList();
            return Ok(_mapper.Map<List<PaymentCard>, List<PaymentCardDto>>(cards));
        }

        [HttpPost]
        public ActionResult<PaymentCardDto> CreateCard([FromBody] CardCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bankAccount = _context.BankAccounts.SingleOrDefault(b => b.Id == model.BankAccountId);
            if (bankAccount == null)
            {
                ModelState.AddModelError(nameof(model.BankAccountId), $"Bank account with id {model.BankAccountId} doesn't exist.");
                return BadRequest(ModelState);
            }

            var visaPaymentCardNumberBuilder = _paymentCardNumberFactory.GetPaymentCardNumberBuilder(IssuingNetwork.Visa);
            var visaPaymentCardNumber = visaPaymentCardNumberBuilder.GeneratePaymentCardNumber(IssuingNetworkSettings.Visa.Length.Sixteen, (int)model.BankAccountId);

            var paymentCard = _mapper.Map<PaymentCard>(visaPaymentCardNumber);
            paymentCard.BankAccountId = (int)model.BankAccountId;

            _context.PaymentCards.Add(paymentCard);
            _context.SaveChanges();

            return Ok(_mapper.Map<PaymentCardDto>(paymentCard));
        }
    }
}
