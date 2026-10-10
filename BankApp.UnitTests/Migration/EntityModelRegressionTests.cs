using System.Linq;
using BankApp.Data;
using BankApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankApp.UnitTests.Migration
{
    [TestClass]
    public class EntityModelRegressionTests
    {
        [TestMethod]
        public void UpgradeDoesNotRequireSchemaChanges()
        {
            using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=unused.invalid;Database=unused;Integrated Security=true").Options);
            var snapshot = context.GetService<IMigrationsAssembly>().ModelSnapshot;
            var initializer = context.GetService<IModelRuntimeInitializer>();
            var oldModel = initializer.Initialize(snapshot.Model, designTime: true);
            var newModel = context.GetService<IDesignTimeModel>().Model;
            var differ = context.GetService<IMigrationsModelDiffer>();
            var changes = differ.GetDifferences(oldModel.GetRelationalModel(), newModel.GetRelationalModel());
            Assert.AreEqual(0, changes.Count, string.Join(", ", changes.Select(c => c.GetType().Name)));
        }

        [TestMethod]
        public void SqlServerModelPreservesMoneyUniqueIbanAndRestrictedCreatorRelationship()
        {
            // Model construction only: no connection, EnsureCreated, migrations or seeding.
            using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=unused.invalid;Database=unused;Integrated Security=true").Options);
            var model = context.Model;
            var account = model.FindEntityType(typeof(BankAccount));
            Assert.AreEqual("BankAccounts", account.GetTableName());
            Assert.AreEqual("decimal(18,2)", account.FindProperty(nameof(BankAccount.Balance)).GetColumnType());
            Assert.AreEqual("decimal(18,2)", account.FindProperty(nameof(BankAccount.DebitLimit)).GetColumnType());
            Assert.IsTrue(account.GetIndexes().Single(i => i.Properties.SingleOrDefault()?.Name == nameof(BankAccount.Iban)).IsUnique);
            Assert.AreEqual(DeleteBehavior.Restrict, account.GetForeignKeys().Single(f => f.Properties.SingleOrDefault()?.Name == nameof(BankAccount.CreatedById)).DeleteBehavior);
            Assert.AreEqual("AspNetUsers", model.FindEntityType(typeof(ApplicationUser)).GetTableName());
            Assert.AreEqual(typeof(int), model.FindEntityType(typeof(ApplicationUser)).FindPrimaryKey().Properties.Single().ClrType);
            Assert.AreEqual("RefreshTokens", model.FindEntityType(typeof(RefreshTokenData)).GetTableName());
            Assert.IsFalse(model.GetEntityTypes().SelectMany(e => e.GetProperties()).Any(p => p.IsShadowProperty() && p.IsForeignKey()),
                "The upgrade must not silently introduce additional shadow foreign keys.");
        }
    }
}
