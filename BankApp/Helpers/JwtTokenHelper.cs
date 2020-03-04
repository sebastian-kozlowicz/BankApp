using BankApp.Interfaces;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BankApp.Helpers
{
    public class JwtTokenHelper
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity claimsIdentity, IJwtFactory jwtFactory, string email, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
        {
            var response = new
            {
                userId = claimsIdentity.Claims.Single(c => c.Type == "userId").Value,
                token = await jwtFactory.GenerateEncodedToken(email, claimsIdentity),
                expiresIn = (int)jwtOptions.ValidFor.TotalSeconds
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}
