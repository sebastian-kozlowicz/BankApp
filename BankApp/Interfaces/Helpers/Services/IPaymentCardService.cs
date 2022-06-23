using System.Collections.Generic;
using System.Threading.Tasks;
using BankApp.Dtos.Card;
using BankApp.Models;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface IPaymentCardService
    {
        Task<PaymentCard> GetPaymentCardAsync(int cardId);
        Task<IEnumerable<PaymentCard>> GetPaymentCardsAsync();
        Task<PaymentCard> CreatePaymentCardAsync(CardCreationDto model);
    }
}