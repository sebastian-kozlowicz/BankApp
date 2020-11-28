using BankApp.Data.EntityConfigurations;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public DbSet<TellerAtBranchHistory> TellerAtBranchHistory { get; set; }
        public DbSet<ManagerAtBranchHistory> ManagerAtBranchHistory { get; set; }
        public DbSet<BankData> BankData { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<BranchAddress> BranchAddresses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankTransfer> BankTransfers { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Headquarters> Headquarters { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Teller> Tellers { get; set; }
        public DbSet<Manager> Managers { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new BankAccountConfiguration());
            builder.ApplyConfiguration(new BranchConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
