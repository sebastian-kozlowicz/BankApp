using BankApp.Interfaces;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BankApp.Helpers
{
    public class JwtTokenHelper
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity claimsIdentity, IJwtFactory jwtFactory, string email, JsonSerializerSettings serializerSettings)
        {
            var response = new
            {
                token = await jwtFactory.GenerateEncodedToken(email, claimsIdentity)
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}
