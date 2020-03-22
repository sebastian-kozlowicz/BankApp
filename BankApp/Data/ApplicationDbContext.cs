﻿using BankApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
             builder.Entity<Administrator>()
                .HasKey(a => a.Id);

            builder.Entity<Administrator>()
                .HasOne(a => a.ApplicationUser)
                .WithOne(a => a.Administrator)
                .HasForeignKey<Administrator>(a => a.Id);

            builder.Entity<Customer>()
                .HasKey(c => c.Id);

            builder.Entity<Customer>()
                .HasOne(c => c.ApplicationUser)
                .WithOne(c => c.Customer)
                .HasForeignKey<Customer>(c => c.Id);

            builder.Entity<Employee>()
                .HasKey(e => e.Id);

            builder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithOne(e => e.Employee)
                .HasForeignKey<Employee>(e => e.Id);

            builder.Entity<BankAccount>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(builder);
        }
    }
}
