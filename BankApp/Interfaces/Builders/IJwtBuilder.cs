using System.Collections.Generic;
using System.Security.Claims;
using BankApp.Models;

namespace BankApp.Interfaces.Builders
{
    public interface IJwtBuilder
    {
        string GenerateEncodedToken(ClaimsIdentity claimsIdentity);
        ClaimsIdentity GenerateClaimsIdentity(ApplicationUser user, IEnumerable<string> roles);
    }
}
