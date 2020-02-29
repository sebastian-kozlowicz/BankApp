using Microsoft.IdentityModel.Tokens;

namespace BankApp.Helpers
{
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
    }
}
