using System.Security.Claims;
using System.Threading.Tasks;
using BankApp.Dtos.Auth;
using BankApp.Interfaces;
using BankApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtBuilder _jwtBuilder;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtBuilder jwtBuilder)
        {
            _userManager = userManager;
            _jwtBuilder = jwtBuilder;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await GetClaimsIdentity(model.Email, model.Password) is var claimsIdentity && claimsIdentity != null)
            {
                var jwt = _jwtBuilder.GenerateEncodedToken(claimsIdentity);

                return new AuthResultDto
                {
                    Token = jwt
                };
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return BadRequest(ModelState);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string email, string password)
        {
            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                if (await _userManager.FindByEmailAsync(email) is var user && user != null)
                {
                    if (await _userManager.CheckPasswordAsync(user, password))
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        return await Task.FromResult(_jwtBuilder.GenerateClaimsIdentity(user, roles));
                    }
                }
            }

            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}