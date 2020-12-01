using BankApp.Enumerators;
using Microsoft.AspNetCore.Identity;
using BankApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp.Data
{
    public static class DataInitializer
    {
        public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, ApplicationDbContext context)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
            SeedBankData(context);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(UserRole.Administrator.ToString()))
                await roleManager.CreateAsync(new IdentityRole<int>(UserRole.Administrator.ToString()));

            if (!await roleManager.RoleExistsAsync(UserRole.Customer.ToString()))
                await roleManager.CreateAsync(new IdentityRole<int>(UserRole.Customer.ToString()));

            if (!await roleManager.RoleExistsAsync(UserRole.Teller.ToString()))
                await roleManager.CreateAsync(new IdentityRole<int>(UserRole.Teller.ToString()));

            if (!await roleManager.RoleExistsAsync(UserRole.Manager.ToString()))
                await roleManager.CreateAsync(new IdentityRole<int>(UserRole.Manager.ToString()));
        }

        private static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@localhost") == null)
            {
                var user = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };

                user.Administrator = new Administrator { Id = user.Id };

                var result = await userManager.CreateAsync(user, "Qwerty1@");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, UserRole.Administrator.ToString());
            }

            if (await userManager.FindByEmailAsync("customer@localhost") == null)
            {
                var user = new ApplicationUser { UserName = "customer@localhost", Email = "customer@localhost" };

                user.Customer = new Customer { Id = user.Id };

                var result = await userManager.CreateAsync(user, "Qwerty1@");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
            }

            if (await userManager.FindByEmailAsync("teller@localhost") == null)
            {
                var user = new ApplicationUser { UserName = "teller@localhost", Email = "teller@localhost" };

                user.Teller = new Teller { Id = user.Id };

                var result = await userManager.CreateAsync(user, "Qwerty1@");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, UserRole.Teller.ToString());
            }

            if (await userManager.FindByEmailAsync("manager@localhost") == null)
            {
                var user = new ApplicationUser { UserName = "manager@localhost", Email = "manager@localhost" };

                user.Manager = new Manager { Id = user.Id };

                var result = await userManager.CreateAsync(user, "Qwerty1@");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, UserRole.Manager.ToString());
            }
        }

        private static void SeedBankData(ApplicationDbContext context)
        {
            var bankDataInDb = context.BankData.ToList();

            if (!bankDataInDb.Any())
            {
                var bankData = new BankData { CountryCode = "PL", NationalBankCode = "1080" };

                context.BankData.Add(bankData);
                context.SaveChanges();
            }
        }
    }
}
