using System.Collections.Generic;
using System.Security.Claims;
using BankApp.Models;
using Microsoft.IdentityModel.Tokens;

namespace BankApp.Interfaces.Helpers.Builders.Auth
{
    public interface IJwtBuilder
    {
        SecurityToken GenerateSecurityToken(ClaimsIdentity claimsIdentity);
        ClaimsIdentity GenerateClaimsIdentity(ApplicationUser user, IEnumerable<string> roles);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
