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
            await SeedBankData(context);
            await SeedBankIdentificationNumberData(context);
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

        private static async Task SeedBankData(ApplicationDbContext context)
        {
            var bankDataInDb = context.BankData.ToList();

            if (!bankDataInDb.Any())
            {
                var bankData = new BankData { CountryCode = "PL", NationalBankCode = "1080" };

                await context.BankData.AddAsync(bankData);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedBankIdentificationNumberData(ApplicationDbContext context)
        {
            var visaBinsInDb = context.BankIdentificationNumberData.Where(bin => bin.IssuingNetwork == IssuingNetwork.Visa).ToList();

            if (!visaBinsInDb.Any())
            {
                var visaBin = new BankIdentificationNumberData { BankIdentificationNumber = 427329, IssuingNetwork = IssuingNetwork.Visa };
                await context.BankIdentificationNumberData.AddAsync(visaBin);
            }

            var mastercardBinsInDb = context.BankIdentificationNumberData.Where(bin => bin.IssuingNetwork == IssuingNetwork.Mastercard).ToList();

            if (!mastercardBinsInDb.Any())
            {
                var mastercardBin = new BankIdentificationNumberData { BankIdentificationNumber = 510918, IssuingNetwork = IssuingNetwork.Mastercard };
                await context.BankIdentificationNumberData.AddAsync(mastercardBin);
            }

            await context.SaveChangesAsync();
        }
    }
}
