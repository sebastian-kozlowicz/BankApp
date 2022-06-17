using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Dtos.Card;
using BankApp.Enumerators;
using BankApp.Exceptions;
using BankApp.Interfaces.Helpers.Factories;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Helpers.Services
{
    public class PaymentCardService : IPaymentCardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPaymentCardNumberFactory _paymentCardNumberFactory;

        public PaymentCardService(ApplicationDbContext context, IMapper mapper,
            IPaymentCardNumberFactory paymentCardNumberFactory)
        {
            _context = context;
            _mapper = mapper;
            _paymentCardNumberFactory = paymentCardNumberFactory;
        }

        public async Task<IEnumerable<PaymentCard>> GetCardsAsync()
        {
            return await _context.PaymentCards.ToListAsync();
        }

        public async Task<PaymentCard> CreateCard(CardCreationDto model)
        {
            var bankAccount = await _context.BankAccounts.SingleOrDefaultAsync(b => b.Id == model.BankAccountId);
            if (bankAccount == null)
                throw new InvalidInputDataException($"Bank account with id {model.BankAccountId} doesn't exist.");

            var visaPaymentCardNumberBuilder =
                _paymentCardNumberFactory.GetPaymentCardNumberBuilder(IssuingNetwork.Visa);
            var visaPaymentCardNumber =
                visaPaymentCardNumberBuilder.GeneratePaymentCardNumber(IssuingNetworkSettings.Visa.Length.Sixteen,
                    (int)model.BankAccountId);

            var paymentCard = _mapper.Map<PaymentCard>(visaPaymentCardNumber);
            paymentCard.BankAccountId = (int)model.BankAccountId;

            await _context.PaymentCards.AddAsync(paymentCard);
            await _context.SaveChangesAsync();

            return paymentCard;
        }
    }
}