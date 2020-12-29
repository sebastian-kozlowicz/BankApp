using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Card;
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
        private readonly ICardNumberFactory _cardNumberFactory;

        public CardsController(ApplicationDbContext context, IMapper mapper, ICardNumberFactory cardNumberFactory)
        {
            _context = context;
            _mapper = mapper;
            _cardNumberFactory = cardNumberFactory;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CardDto>> GetCards()
        {
            var cards = _context.Cards.ToList();
            return Ok(_mapper.Map<List<Card>, List<CardDto>>(cards));
        }

        [HttpPost]
        public ActionResult<CardDto> CreateCard([FromBody] CardCreationDto model)
        {
            return Ok();
        }
    }
}
