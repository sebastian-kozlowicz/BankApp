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
            if (!roleManager.RoleExistsAsync("Administrator").Result)
                await roleManager.CreateAsync(new IdentityRole("Administrator"));

            if (!roleManager.RoleExistsAsync("Customer").Result)
                await roleManager.CreateAsync(new IdentityRole("Customer"));

            if (!roleManager.RoleExistsAsync("Employee").Result)
                await roleManager.CreateAsync(new IdentityRole("Employee"));

            if (!roleManager.RoleExistsAsync("Manager").Result)
                await roleManager.CreateAsync(new IdentityRole("Manager"));
        }
    }
}
