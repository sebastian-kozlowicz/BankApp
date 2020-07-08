using BankApp.Enumerators;
using Microsoft.AspNetCore.Identity;
using BankApp.Models;
using System.Linq;

namespace BankApp.Data
{
    public static class DataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, ApplicationDbContext context)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedBankData(context);
        }

        private static void SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            if (!roleManager.RoleExistsAsync(UserRole.Administrator.ToString()).Result)
                _ = roleManager.CreateAsync(new IdentityRole<int>(UserRole.Administrator.ToString())).Result;

            if (!roleManager.RoleExistsAsync(UserRole.Customer.ToString()).Result)
                _ = roleManager.CreateAsync(new IdentityRole<int>(UserRole.Customer.ToString())).Result;

            if (!roleManager.RoleExistsAsync(UserRole.Employee.ToString()).Result)
                _ = roleManager.CreateAsync(new IdentityRole<int>(UserRole.Employee.ToString())).Result;

            if (!roleManager.RoleExistsAsync(UserRole.Manager.ToString()).Result)
                _ = roleManager.CreateAsync(new IdentityRole<int>(UserRole.Manager.ToString())).Result;
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("admin@localhost").Result == null)
            {
                var user = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };

                user.Administrator = new Administrator() { Id = user.Id };

                var result = userManager.CreateAsync(user, "Qwerty1@").Result;

                if (result.Succeeded)
                    _ = userManager.AddToRoleAsync(user, UserRole.Administrator.ToString()).Result;
            }

            if (userManager.FindByEmailAsync("customer@localhost").Result == null)
            {
                var user = new ApplicationUser { UserName = "customer@localhost", Email = "customer@localhost" };

                user.Customer = new Customer() { Id = user.Id };

                var result = userManager.CreateAsync(user, "Qwerty1@").Result;

                if (result.Succeeded)
                {
                    _ = userManager.AddToRoleAsync(user, UserRole.Customer.ToString()).Result;
                }
            }

            if (userManager.FindByEmailAsync("employee@localhost").Result == null)
            {
                var user = new ApplicationUser { UserName = "employee@localhost", Email = "employee@localhost" };

                user.Employee = new Employee() { Id = user.Id };

                var result = userManager.CreateAsync(user, "Qwerty1@").Result;

                if (result.Succeeded)
                {
                    _ = userManager.AddToRoleAsync(user, UserRole.Employee.ToString()).Result;
                }
            }

            if (userManager.FindByEmailAsync("manager@localhost").Result == null)
            {
                var user = new ApplicationUser { UserName = "manager@localhost", Email = "manager@localhost" };

                user.Employee = new Employee() { Id = user.Id };

                var result = userManager.CreateAsync(user, "Qwerty1@").Result;

                if (result.Succeeded)
                {
                    _ = userManager.AddToRoleAsync(user, UserRole.Manager.ToString()).Result;
                }
            }
        }

        private static void SeedBankData(ApplicationDbContext context)
        {
            var banDataInDb = context.BankData.ToList();

            if (!banDataInDb.Any())
            {
                var bankData = new BankData { CountryCode = "PL", NationalBankCode = "1080" };

                context.BankData.Add(bankData);
                context.SaveChanges();
            }
        }
    }
}
