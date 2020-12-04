using System;
using System.Linq;
using AutoMapper;
using BankApp.Controllers;
using BankApp.Data;
using BankApp.Dtos.Address;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Mapping;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
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
        private readonly Branch _branch = new Branch
        {
            Id = 1,
            BranchCode = "000",
            BranchAddress = new BranchAddress
            {
                Id = 1,
                Country = "Poland",
                City = "Poznań",
                Street = "Półwiejska",
                HouseNumber = "1",
                ApartmentNumber = "2",
                PostalCode = "61-001"
            }
        };

        private ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.Branches.Add(_branch);
            context.SaveChanges();

            return context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = GetMockContext();
            _branchesController = new BranchesController(_context, _mapper);
        }

        [TestMethod]
        public void GetBranch_Should_ReturnBranchDto_When_BranchIsFound()
        {
            var okResult = _branchesController.GetBranch(1).Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(BranchDto));

            var branchDto = okResult.Value as BranchDto;
            Assert.IsNotNull(branchDto);
            Assert.AreEqual(_branch.Id, branchDto.Id);
            Assert.AreEqual(_branch.BranchCode, branchDto.BranchCode);
            Assert.AreEqual(_branch.BranchAddress.Id, branchDto.BranchAddress.Id);
            Assert.AreEqual(_branch.BranchAddress.Country, branchDto.BranchAddress.Country);
            Assert.AreEqual(_branch.BranchAddress.City, branchDto.BranchAddress.City);
            Assert.AreEqual(_branch.BranchAddress.Street, branchDto.BranchAddress.Street);
            Assert.AreEqual(_branch.BranchAddress.HouseNumber, branchDto.BranchAddress.HouseNumber);
            Assert.AreEqual(_branch.BranchAddress.ApartmentNumber, branchDto.BranchAddress.ApartmentNumber);
            Assert.AreEqual(_branch.BranchAddress.PostalCode, branchDto.BranchAddress.PostalCode);
        }

        [TestMethod]
        public void GetBranch_Should_ReturnNotFound_When_BranchIsNotFound()
        {
            var notFoundResult = _branchesController.GetBranch(999);

            Assert.IsNotNull(notFoundResult);
            Assert.IsInstanceOfType(notFoundResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateBranchWithAddress_Should_CreateBranchWithAddress_And_ReturnBranchDto_When_ModelStateIsValid()
        {
            // Arrange
            var branchCreation = new BranchWithAddressCreationDto
            {
                Branch = new BranchCreationDto
                {
                    BranchCode = "001"
                },
                Address = new AddressCreationDto
                {
                    Country = "United States",
                    City = "New York",
                    Street = "Glenwood Ave",
                    HouseNumber = "10",
                    ApartmentNumber = "11",
                    PostalCode = "10028"
                }
            };

            // Act
            var createdAtRouteResult = _branchesController.CreateBranchWithAddress(branchCreation).Result as CreatedAtRouteResult;

            // Assert
            Assert.IsNotNull(createdAtRouteResult);
            Assert.IsInstanceOfType(createdAtRouteResult.Value, typeof(BranchDto));

            var branchDto = createdAtRouteResult.Value as BranchDto;
            Assert.IsNotNull(branchDto);
            Assert.AreEqual(branchCreation.Branch.BranchCode, branchDto.BranchCode);
            Assert.AreEqual(branchCreation.Address.Country, branchDto.BranchAddress.Country);
            Assert.AreEqual(branchCreation.Address.City, branchDto.BranchAddress.City);
            Assert.AreEqual(branchCreation.Address.Street, branchDto.BranchAddress.Street);
            Assert.AreEqual(branchCreation.Address.HouseNumber, branchDto.BranchAddress.HouseNumber);
            Assert.AreEqual(branchCreation.Address.ApartmentNumber, branchDto.BranchAddress.ApartmentNumber);
            Assert.AreEqual(branchCreation.Address.PostalCode, branchDto.BranchAddress.PostalCode);

            var branchFromDb = _context.Branches.SingleOrDefault(b => b.Id == branchDto.Id);
            Assert.IsNotNull(branchFromDb);
            Assert.AreEqual(branchCreation.Branch.BranchCode, branchFromDb.BranchCode);
            Assert.AreEqual(branchCreation.Address.Country, branchFromDb.BranchAddress.Country);
            Assert.AreEqual(branchCreation.Address.City, branchFromDb.BranchAddress.City);
            Assert.AreEqual(branchCreation.Address.Street, branchFromDb.BranchAddress.Street);
            Assert.AreEqual(branchCreation.Address.HouseNumber, branchFromDb.BranchAddress.HouseNumber);
            Assert.AreEqual(branchCreation.Address.ApartmentNumber, branchFromDb.BranchAddress.ApartmentNumber);
            Assert.AreEqual(branchCreation.Address.PostalCode, branchFromDb.BranchAddress.PostalCode);
        }

        [TestMethod]
        public void CreateBranchWithAddress_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var branchCreation = new BranchWithAddressCreationDto();
            _branchesController.ModelState.AddModelError("Branch", "The Branch field is required.");
            _branchesController.ModelState.AddModelError("Address", "The Address field is required.");

            // Act
            var result = _branchesController.CreateBranchWithAddress(branchCreation);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

            var error = badRequestResult.Value as SerializableError;
            Assert.IsNotNull(error);
            Assert.IsTrue(error.ContainsKey("Branch"));
            Assert.IsTrue(error.ContainsKey("Address"));

            var currencyErrorValues = error["Branch"] as string[];
            Assert.IsNotNull(currencyErrorValues);
            Assert.IsTrue(currencyErrorValues.Single() == "The Branch field is required.");

            var customerIdErrorValues = error["Address"] as string[];
            Assert.IsNotNull(customerIdErrorValues);
            Assert.IsTrue(customerIdErrorValues.Single() == "The Address field is required.");
        }
    }
}
