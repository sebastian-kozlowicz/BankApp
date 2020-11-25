using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BankApp.Configuration;
using BankApp.Policies.Requirement;
using Microsoft.AspNetCore.Authorization;

namespace BankApp.Policies.Handlers
{
    public class UserIdRequirementHandler : AuthorizationHandler<UserIdRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIdRequirement requirement)
        {
            var currentUserId = context.User.FindFirst(CustomClaimTypes.UserId)?.Value;
            if (currentUserId == null)
                return Task.CompletedTask;

            if (Regex.IsMatch(currentUserId, @"^\d+$"))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
