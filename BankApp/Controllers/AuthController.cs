using System.Threading.Tasks;
using BankApp.Dtos.Auth;
using BankApp.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Logs user in
        /// </summary>
        /// <param name="model">Login credentials</param>
        /// <returns>JWT with refresh token</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _authService.LoginAsync(model);
        }

        /// <summary>
        /// Refreshes expired JWT
        /// </summary>
        /// <param name="model">Expired JWT with refresh token</param>
        /// <returns>JWT with refresh token</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPost]
        [AllowAnonymous]
        [Route("refresh")]
        public async Task<ActionResult<AuthResultDto>> Refresh([FromBody] RefreshTokenRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await _authService.RefreshTokenAsync(model.Token, model.RefreshToken);
        }
    }
}