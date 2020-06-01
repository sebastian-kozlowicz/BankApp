using BankApp.Enumerators;
using Microsoft.AspNetCore.Identity;
using BankApp.Models;
using System.Linq;

namespace BankApp.Data
{
    public static class DataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedBankData(context);
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(UserRoles.Administrator.ToString()).Result)
                _ = roleManager.CreateAsync(new IdentityRole(UserRoles.Administrator.ToString())).Result;

            if (!roleManager.RoleExistsAsync(UserRoles.Customer.ToString()).Result)
                _ = roleManager.CreateAsync(new IdentityRole(UserRoles.Customer.ToString())).Result;

            if (!roleManager.RoleExistsAsync(UserRoles.Employee.ToString()).Result)
                _ = roleManager.CreateAsync(new IdentityRole(UserRoles.Employee.ToString())).Result;

            if (!roleManager.RoleExistsAsync(UserRoles.Manager.ToString()).Result)
                _ = roleManager.CreateAsync(new IdentityRole(UserRoles.Manager.ToString())).Result;
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("admin@localhost").Result == null)
            {
                var user = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };

                user.Administrator = new Administrator() { Id = user.Id };

                var result = userManager.CreateAsync(user, "Qwerty1@").Result;

                if (result.Succeeded)
                    _ = userManager.AddToRoleAsync(user, UserRoles.Administrator.ToString()).Result;
            }

            if (userManager.FindByEmailAsync("customer@localhost").Result == null)
            {
                var user = new ApplicationUser { UserName = "customer@localhost", Email = "customer@localhost" };

                user.Customer = new Customer() { Id = user.Id };

                var result = userManager.CreateAsync(user, "Qwerty1@").Result;

                if (result.Succeeded)
                {
                    _ = userManager.AddToRoleAsync(user, UserRoles.Customer.ToString()).Result;
                }
            }

            if (userManager.FindByEmailAsync("employee@localhost").Result == null)
            {
                var user = new ApplicationUser { UserName = "employee@localhost", Email = "employee@localhost" };

                user.Employee = new Employee() { Id = user.Id };

                var result = userManager.CreateAsync(user, "Qwerty1@").Result;

                if (result.Succeeded)
                {
                    _ = userManager.AddToRoleAsync(user, UserRoles.Employee.ToString()).Result;
                }
            }

            if (userManager.FindByEmailAsync("manager@localhost").Result == null)
            {
                var user = new ApplicationUser { UserName = "manager@localhost", Email = "manager@localhost" };

                user.Employee = new Employee() { Id = user.Id };

                var result = userManager.CreateAsync(user, "Qwerty1@").Result;

                if (result.Succeeded)
                {
                    _ = userManager.AddToRoleAsync(user, UserRoles.Manager.ToString()).Result;
                }
            }
        }

        private static void SeedBankData(ApplicationDbContext context)
        {
            var banDataInDb = context.BankData.ToList();

            if (!banDataInDb.Any())
            {
                var bankData = new BankData { CountryCode = "PL", NationalBankCode = 1080 };

                context.BankData.Add(bankData);
                context.SaveChanges();
            }
        }
    }
}
