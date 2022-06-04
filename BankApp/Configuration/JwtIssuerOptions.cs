using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BankApp.Configuration
{
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public DateTime Expiration => IssuedAt.Add(ValidFor);
        public DateTime NotBefore => DateTime.UtcNow;
        public DateTime IssuedAt => DateTime.UtcNow;
        public TimeSpan ValidFor { get; set; }
        public string Jti => Guid.NewGuid().ToString();
        public string SecretKey { get; set; }
        public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.ASCII.GetBytes(SecretKey));
        public SigningCredentials SigningCredentials => new(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
    }
}