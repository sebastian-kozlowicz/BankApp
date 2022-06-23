using System.Collections.Generic;
using System.Threading.Tasks;
using BankApp.Dtos.Card;
using BankApp.Models;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface IPaymentCardService
    {
        Task<IEnumerable<PaymentCard>> GetCardsAsync();
        Task<PaymentCard> CreateCardAsync(CardCreationDto model);
    }
}