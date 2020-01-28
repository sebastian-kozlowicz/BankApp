using System.Threading.Tasks;
using BankApp.Models;
using BankApp.ViewModels.Account;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IIdentityServerInteractionService interaction, IAuthenticationSchemeProvider schemeProvider, IClientStore clientStore, IEventService events)
        {
            _userManager = userManager;
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore;
            _events = events;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("api/{controller}")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = model.Name, Surname = model.Surname };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.Name));
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));

            return Ok(user);
        }
    }
}