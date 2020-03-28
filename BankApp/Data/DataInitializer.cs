using BankApp.Enumerators;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BankApp.Data
{
    public class DataInitializer
    {
        public static async Task SeedData(RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(UserRoles.Administrator.ToString()).Result)
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Administrator.ToString()));

            if (!roleManager.RoleExistsAsync(UserRoles.Customer.ToString()).Result)
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Customer.ToString()));

            if (!roleManager.RoleExistsAsync(UserRoles.Employee.ToString()).Result)
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Employee.ToString()));

            if (!roleManager.RoleExistsAsync(UserRoles.Manager.ToString()).Result)
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Manager.ToString()));
        }
    }
}
