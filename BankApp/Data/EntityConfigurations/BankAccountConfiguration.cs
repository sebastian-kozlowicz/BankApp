using BankApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApp.Data.EntityConfigurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder
                .Property(b => b.Balance)
                .HasColumnType("decimal(18,2)");

            builder
                .Property(b => b.DebitLimit)
                .HasColumnType("decimal(18,2)");

            builder
                .HasIndex(b => b.Iban)
                .IsUnique();
        }
    }
}