using BankApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApp.Data.EntityConfigurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
                .HasOne(a => a.Administrator)
                .WithOne(a => a.ApplicationUser)
                .HasForeignKey<Administrator>(a => a.Id);

            builder
                .HasOne(a => a.Customer)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<Customer>(c => c.Id);

            builder
                .HasOne(a => a.Teller)
                .WithOne(e => e.ApplicationUser)
                .HasForeignKey<Teller>(e => e.Id);

            builder
                .HasOne(a => a.Manager)
                .WithOne(m => m.ApplicationUser)
                .HasForeignKey<Manager>(m => m.Id);

            builder
                .HasOne(a => a.UserAddress)
                .WithOne(ua => ua.ApplicationUser)
                .HasForeignKey<UserAddress>(ua => ua.Id);

            builder
                .HasMany(a => a.CreatedBankAccounts)
                .WithOne(ba => ba.CreatedBy)
                .HasForeignKey(ba => ba.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.AssignedTellersToBranchHistory)
                .WithOne(eat => eat.AssignedBy)
                .HasForeignKey(eat => eat.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.ExpelledTellersFromBranchHistory)
                .WithOne(eat => eat.ExpelledBy)
                .HasForeignKey(eat => eat.ExpelledById);

            builder
                .HasMany(a => a.AssignedManagersToBranchHistory)
                .WithOne(mat => mat.AssignedBy)
                .HasForeignKey(mat => mat.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(a => a.ExpelledManagersFromBranchHistory)
                .WithOne(mat => mat.ExpelledBy)
                .HasForeignKey(mat => mat.ExpelledById);
        }
    }
}