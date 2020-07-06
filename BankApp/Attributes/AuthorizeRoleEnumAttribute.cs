using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace BankApp.Attributes
{
    public class AuthorizeRoleEnumAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleEnumAttribute(params object[] roles)
        {
            if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("roles");

            Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
        }
    }
}
