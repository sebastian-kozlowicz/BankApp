using AutoMapper;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Dtos.Card;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPaymentCardNumberFactory _paymentCardNumberFactory;

        public CardsController(ApplicationDbContext context, IMapper mapper, IPaymentCardNumberFactory paymentCardNumberFactory)
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
            var visaPaymentCardNumberBuilder = _paymentCardNumberFactory.GetPaymentCardNumberBuilder(IssuingNetwork.Visa);
            var visaPaymentCardNumber = visaPaymentCardNumberBuilder.GeneratePaymentCardNumber(VisaAcceptedLength.Sixteen);

            return Ok();
        }
    }
}
