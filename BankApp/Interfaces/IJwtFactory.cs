using BankApp.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BankApp.Interfaces
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string email, ClaimsIdentity claimsIdentity);
        ClaimsIdentity GenerateClaimsIdentity(ApplicationUser user, IList<string> roles);
    }
}
