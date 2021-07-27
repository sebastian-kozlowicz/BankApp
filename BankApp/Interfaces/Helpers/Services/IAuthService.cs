using System.Threading.Tasks;
using BankApp.Dtos.Auth;

namespace BankApp.Interfaces.Helpers.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto model);
        Task<AuthResultDto> RefreshTokenAsync(string token, string refreshToken);
    }
}