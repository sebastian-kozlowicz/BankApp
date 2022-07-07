using System.Threading.Tasks;
using BankApp.Dtos.BankTransfer;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface IBankTransferService
    {
        Task<bool> CreateBankTransferAsync(BankTransferCreationDto bankTransferCreationDto);
    }
}