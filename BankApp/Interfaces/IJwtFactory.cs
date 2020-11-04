using BankApp.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace BankApp.Interfaces
{
    public interface IJwtFactory
    {
        string GenerateEncodedToken(ClaimsIdentity claimsIdentity);
        ClaimsIdentity GenerateClaimsIdentity(ApplicationUser user, IEnumerable<string> roles);
    }
}
