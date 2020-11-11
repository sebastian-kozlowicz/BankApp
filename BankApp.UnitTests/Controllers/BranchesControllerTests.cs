using System;
using AutoMapper;
using BankApp.Controllers;
using BankApp.Data;
using BankApp.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankApp.UnitTests.Controllers
{
    [TestClass]
    public class BranchesControllerTests
    {
        private BranchesController _branchesController;
        private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        private ApplicationDbContext _context;

        private static ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.SaveChanges();

            return context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = GetMockContext();
            _branchesController = new BranchesController(_context, _mapper);
        }
    }
}
