using BankApp.Interfaces;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BankApp.Helpers
{
    public class JwtTokenHelper
    {
        public static string GenerateJwt(ClaimsIdentity claimsIdentity, IJwtFactory jwtFactory, string email, JsonSerializerSettings serializerSettings)
        {
            var response = new
            {
                token =  jwtFactory.GenerateEncodedToken(email, claimsIdentity)
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}
