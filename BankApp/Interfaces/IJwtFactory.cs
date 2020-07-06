using BankApp.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace BankApp.Interfaces
{
    public interface IJwtFactory
    {
        string GenerateEncodedToken(string email, ClaimsIdentity claimsIdentity);
        ClaimsIdentity GenerateClaimsIdentity(ApplicationUser user, IList<string> roles);
    }
}
