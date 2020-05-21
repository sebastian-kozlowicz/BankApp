using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Dtos.Auth;
using BankApp.Helpers;
using BankApp.Interfaces;
using BankApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtFactory _jwtFactory;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtFactory jwtFactory)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await GetClaimsIdentity(model.Email, model.Password) is ClaimsIdentity claimsIdentity && claimsIdentity != null)
            {
                var jwt = await JwtTokenHelper.GenerateJwt(claimsIdentity, _jwtFactory, model.Email, new JsonSerializerSettings { Formatting = Formatting.Indented });

                return Ok(jwt);
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return BadRequest(ModelState);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(password))
            {
                if (await _userManager.FindByEmailAsync(email) is var user)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (await _userManager.CheckPasswordAsync(user, password))
                        return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(user, roles));
                }
            }

            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}