using BankApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApp.Data.EntityConfigurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder
                .HasOne(b => b.Headquarters)
                .WithOne(h => h.Branch)
                .HasForeignKey<Headquarters>(h => h.Id);

            builder
                .HasOne(b => b.BranchAddress)
                .WithOne(ba => ba.Branch)
                .HasForeignKey<BranchAddress>(ba => ba.Id);
        }
    }
}