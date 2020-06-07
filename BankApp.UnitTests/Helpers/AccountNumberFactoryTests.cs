using BankApp.Data;
using BankApp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BankApp.UnitTests.Helpers
{
    [TestClass]
    public class AccountNumberFactoryTests
    {
        private ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            return context;
        }

        private AccountNumberFactory accountNumberFactory;

        [TestInitialize]
        public void ClassInitalize()
        {
            accountNumberFactory = new AccountNumberFactory(GetMockContext());
        }

        [TestMethod]
        public void Test()
        {

        }
    }
}
