using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BankApp.Attributes
{
    public class AuthorizeRoleEnumAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleEnumAttribute(params object[] roles)
        {
            if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("Base type is not Enum");

            Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
        }
    }
}