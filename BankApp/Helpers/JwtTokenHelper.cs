using BankApp.Interfaces;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BankApp.Helpers
{
    public class JwtTokenHelper
    {
        public static string GenerateJwt(ClaimsIdentity claimsIdentity, IJwtFactory jwtFactory, JsonSerializerSettings serializerSettings)
        {
            var response = new
            {
                token = jwtFactory.GenerateEncodedToken(claimsIdentity)
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}
