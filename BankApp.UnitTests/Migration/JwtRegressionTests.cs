using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BankApp.Configuration;
using BankApp.Helpers.Builders.Auth;
using BankApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankApp.UnitTests.Migration
{
    [TestClass]
    public class JwtRegressionTests
    {
        private readonly JwtIssuerOptions _options = new()
        {
            Issuer = "bankapp-tests", Audience = "bankapp-client-tests",
            SecretKey = "test-only-key-at-least-32-bytes-long".PadRight(64, 'x'), ValidFor = TimeSpan.FromMinutes(5)
        };

        private JwtBuilder CreateBuilder() => new(Options.Create(_options), new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, IssuerSigningKey = _options.SymmetricSecurityKey,
            ValidateIssuer = true, ValidIssuer = _options.Issuer,
            ValidateAudience = true, ValidAudience = _options.Audience,
            ValidateLifetime = true, ClockSkew = TimeSpan.Zero
        });

        [TestMethod]
        public void GeneratedTokenRoundTripsUserIdEmailAndRoles()
        {
            var builder = CreateBuilder();
            var identity = builder.GenerateClaimsIdentity(new ApplicationUser { Id = 17, Email = "test@example.invalid" }, new[] { "Customer" });
            var token = new JwtSecurityTokenHandler().WriteToken(builder.GenerateSecurityToken(identity));
            var principal = builder.GetPrincipalFromToken(token);
            Assert.IsNotNull(principal);
            Assert.AreEqual("17", principal.FindFirst(CustomClaimTypes.UserId).Value);
            Assert.AreEqual("test@example.invalid", principal.FindFirst(ClaimTypes.NameIdentifier).Value);
            Assert.IsTrue(principal.IsInRole("Customer"));
        }

        [TestMethod]
        [DataRow("expired")]
        [DataRow("issuer")]
        [DataRow("audience")]
        [DataRow("signature")]
        [DataRow("algorithm")]
        public void InvalidTokenIsRejected(string failure)
        {
            var now = DateTime.UtcNow;
            var credentials = _options.SigningCredentials;
            if (failure == "signature")
                credentials = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes("different-test-only-key-at-least-32-bytes")), SecurityAlgorithms.HmacSha256);
            if (failure == "algorithm")
                credentials = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_options.SecretKey.PadRight(64, 'x'))), SecurityAlgorithms.HmacSha512);
            var token = new JwtSecurityToken(
                failure == "issuer" ? "wrong-issuer" : _options.Issuer,
                failure == "audience" ? "wrong-audience" : _options.Audience,
                new[] { new Claim(CustomClaimTypes.UserId, "17") },
                now.AddMinutes(-10), failure == "expired" ? now.AddMinutes(-5) : now.AddMinutes(5), credentials);
            Assert.IsNull(CreateBuilder().GetPrincipalFromToken(new JwtSecurityTokenHandler().WriteToken(token)));
        }

        [TestMethod]
        public void MalformedTokenIsRejected()
        {
            Assert.IsNull(CreateBuilder().GetPrincipalFromToken("not-a-jwt"));
        }
    }
}
