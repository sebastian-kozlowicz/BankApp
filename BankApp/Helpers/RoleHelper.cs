using BankApp.Enumerators;
using System.Collections.Generic;

namespace BankApp.Helpers
{
    public static class RoleHelper
    {
        public static bool IsUserInRole(IList<string> roles, UserRole userRoles)
        {
            return roles.Contains(userRoles.ToString());
        }
    }
}
