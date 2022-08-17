﻿using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Configuration;
using BankApp.Controllers;
using BankApp.Dtos.Address;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Interfaces.Helpers.Services;
using BankApp.Mapping;
using BankApp.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BankApp.UnitTests.Controllers
{
    [TestClass]
    public class BranchesControllerTests
    {
        private readonly Branch _branch = new()
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

        private readonly BranchDto _branchDto = new()
        {
            Id = 1,
            BranchCode = "000",
            BranchAddress = new AddressDto
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

        private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
        private Mock<IBranchService> _branchServiceMock;
        private BranchesController _sut;


        [TestInitialize]
        public void TestInitialize()
        {
            _branchServiceMock = new Mock<IBranchService>();
            _sut = new BranchesController(_branchServiceMock.Object, _mapper);
        }

        [TestMethod]
        public async Task GetBranchAsync_Should_ReturnBranchDto_When_BranchIsFound()
        {
            // Arrange
            _branchServiceMock.Setup(s => s.GetBranchAsync(It.IsAny<int>())).ReturnsAsync(_branch);

            // Act
            var result = await _sut.GetBranchAsync(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            okResult.Value.Should().BeEquivalentTo(_branchDto);
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetBranchAsync_Should_ReturnNotFound_When_BranchIsNotFound()
        {
            // Arrange
            _branchServiceMock.Setup(s => s.GetBranchAsync(It.IsAny<int>())).ReturnsAsync((Branch)null);

            // Act
            var result = await _sut.GetBranchAsync(999);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            notFoundResult.Should().NotBeNull();

            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task
            CreateBranchWithAddressAsync_Should_CreateBranchWithAddress_And_ReturnBranchDto_When_ModelStateIsValid()
        {
            // Arrange
            var branchCreation = new BranchWithAddressCreationDto
            {
                Branch = new BranchCreationDto
                {
                    BranchCode = "002"
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

            var branch = new Branch
            {
                BranchCode = branchCreation.Branch.BranchCode,
                BranchAddress = new BranchAddress
                {
                    Country = branchCreation.Address.Country,
                    City = branchCreation.Address.City,
                    Street = branchCreation.Address.Street,
                    HouseNumber = branchCreation.Address.HouseNumber,
                    ApartmentNumber = branchCreation.Address.ApartmentNumber,
                    PostalCode = branchCreation.Address.PostalCode
                }
            };

            var expectedBranchDto = new BranchDto
            {
                BranchCode = branchCreation.Branch.BranchCode,
                BranchAddress = new AddressDto
                {
                    Country = branchCreation.Address.Country,
                    City = branchCreation.Address.City,
                    Street = branchCreation.Address.Street,
                    HouseNumber = branchCreation.Address.HouseNumber,
                    ApartmentNumber = branchCreation.Address.ApartmentNumber,
                    PostalCode = branchCreation.Address.PostalCode
                }
            };

            _branchServiceMock.Setup(s => s.CreateBranchWithAddressAsync(It.IsAny<BranchWithAddressCreationDto>()))
                .ReturnsAsync(branch);

            // Act
            var result = await _sut.CreateBranchWithAddressAsync(branchCreation);

            // Assert
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            createdAtRouteResult.Should().NotBeNull();

            createdAtRouteResult.Value.Should().BeEquivalentTo(expectedBranchDto);
            createdAtRouteResult.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task CreateBranchWithAddressAsync_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var branchCreation = new BranchWithAddressCreationDto();
            _sut.ModelState.AddModelError(nameof(branchCreation.Branch),
                $"The {nameof(branchCreation.Branch)} field is required.");
            _sut.ModelState.AddModelError(nameof(branchCreation.Address),
                $"The {nameof(branchCreation.Address)} field is required.");

            var expectedResult = new SerializableError
            {
                {
                    nameof(branchCreation.Branch),
                    new[] { $"The {nameof(branchCreation.Branch)} field is required." }
                },
                {
                    nameof(branchCreation.Address),
                    new[] { $"The {nameof(branchCreation.Address)} field is required." }
                }
            };

            // Act
            var result = await _sut.CreateBranchWithAddressAsync(branchCreation);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task AssignTellerToBranchAsync_Should_ReturnOkObjectResult_When_ModelStateIsValid()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 2,
                BranchId = 1
            };

            var currentUser = new ApplicationUser { Id = 1 };
            var claims = new List<Claim> { new(CustomClaimTypes.UserId, currentUser.Id.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            _sut.ControllerContext = context;

            // Act
            var result = await _sut.AssignTellerToBranchAsync(workerAtBranch);

            // Assert
            var createdAtRouteResult = result as OkResult;
            createdAtRouteResult.Should().NotBeNull();

            createdAtRouteResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task AssignTellerToBranchAsync_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto();

            _sut.ModelState.AddModelError(nameof(workerAtBranch.WorkerId), $"The {nameof(workerAtBranch.WorkerId)} field is required.");
            _sut.ModelState.AddModelError(nameof(workerAtBranch.BranchId), $"The {nameof(workerAtBranch.BranchId)} field is required.");

            var expectedResult = new SerializableError
            {
                {
                    nameof(workerAtBranch.WorkerId),
                    new[] { $"The {nameof(workerAtBranch.WorkerId)} field is required." }
                },
                {
                    nameof(workerAtBranch.BranchId),
                    new[] { $"The {nameof(workerAtBranch.BranchId)} field is required." }
                }
            };

            // Act
            var result = await _sut.AssignTellerToBranchAsync(workerAtBranch);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().BeEquivalentTo(expectedResult);
        }

        //[TestMethod]
        //public void AssignTellerToBranch_Should_ReturnBadRequest_When_TellerNotExist()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 999,
        //        BranchId = 1
        //    };

        //    // Act
        //    var badRequestResult = _sut.AssignTellerToBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.WorkerId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.WorkerId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Teller with id {workerAtBranch.WorkerId} doesn't exist.");
        //}

        //[TestMethod]
        //public void AssignTellerToBranch_Should_ReturnBadRequest_When_BranchNotExist()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 2,
        //        BranchId = 999
        //    };

        //    // Act
        //    var badRequestResult = _sut.AssignTellerToBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Branch with id {workerAtBranch.BranchId} doesn't exist.");
        //}

        //[TestMethod]
        //public void AssignTellerToBranch_Should_ReturnBadRequest_When_TellerIsAlreadyAssignedToBranch()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 3,
        //        BranchId = 1
        //    };

        //    // Act
        //    var badRequestResult = _sut.AssignTellerToBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Teller with id {workerAtBranch.WorkerId} is currently assigned to branch with id {_secondBranch.Id}.");
        //}

        //[TestMethod]
        //public void AssignManagerToBranch_Should_SetWorkAtIdPropertyToSuppliedBranchId_And_CreateManagerAtBranchHistory_And_ReturnOkObjectResult_When_ModelStateIsValid()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 4,
        //        BranchId = 1
        //    };

        //    var currentUser = new ApplicationUser { Id = 1 };
        //    var claims = new List<Claim> { new Claim(CustomClaimTypes.UserId, currentUser.Id.ToString()) };
        //    var identity = new ClaimsIdentity(claims);
        //    var claimsPrincipal = new ClaimsPrincipal(identity);
        //    var context = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = claimsPrincipal
        //        }
        //    };

        //    _sut.ControllerContext = context;

        //    // Act
        //    var okResult = _sut.AssignManagerToBranch(workerAtBranch) as OkResult;

        //    // Assert
        //    Assert.IsNotNull(okResult);

        //    var managerFromDb = _context.Managers.SingleOrDefault(m => m.Id == workerAtBranch.WorkerId);
        //    Assert.IsNotNull(managerFromDb);
        //    Assert.AreEqual(workerAtBranch.BranchId, managerFromDb.WorkAtId);

        //    var managerAtBranchFromDb = _context.ManagerAtBranchHistory.Where(m => m.ManagerId == workerAtBranch.WorkerId).ToList().LastOrDefault();
        //    Assert.IsNotNull(managerAtBranchFromDb);
        //    Assert.IsNotNull(managerAtBranchFromDb.AssignDate);
        //    Assert.IsNull(managerAtBranchFromDb.ExpelDate);
        //    Assert.IsNull(managerAtBranchFromDb.ExpelledById);
        //    Assert.AreEqual(currentUser.Id, managerAtBranchFromDb.AssignedById);
        //    Assert.AreEqual(workerAtBranch.BranchId, managerAtBranchFromDb.BranchId);
        //    Assert.AreEqual(workerAtBranch.WorkerId, managerAtBranchFromDb.ManagerId);
        //}

        //[TestMethod]
        //public void AssignManagerToBranch_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto();

        //    _sut.ModelState.AddModelError(nameof(workerAtBranch.WorkerId), $"The {nameof(workerAtBranch.WorkerId)} field is required.");
        //    _sut.ModelState.AddModelError(nameof(workerAtBranch.BranchId), $"The {nameof(workerAtBranch.BranchId)} field is required.");

        //    // Act
        //    var badRequestResult = _sut.AssignManagerToBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.WorkerId)));
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.WorkerId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"The {nameof(workerAtBranch.WorkerId)} field is required.");

        //    var branchIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(branchIdErrorValues);
        //    Assert.IsTrue(branchIdErrorValues.Single() == $"The {nameof(workerAtBranch.BranchId)} field is required.");
        //}

        //[TestMethod]
        //public void AssignManagerToBranch_Should_ReturnBadRequest_When_ManagerNotExist()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 999,
        //        BranchId = 1
        //    };

        //    // Act
        //    var badRequestResult = _sut.AssignManagerToBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.WorkerId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.WorkerId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Manager with id {workerAtBranch.WorkerId} doesn't exist.");
        //}

        //[TestMethod]
        //public void AssignManagerToBranch_Should_ReturnBadRequest_When_BranchNotExist()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 4,
        //        BranchId = 999
        //    };

        //    // Act
        //    var badRequestResult = _sut.AssignManagerToBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Branch with id {workerAtBranch.BranchId} doesn't exist.");
        //}

        //[TestMethod]
        //public void AssignManagerToBranch_Should_ReturnBadRequest_When_ManagerIsAlreadyAssignedToBranch()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 5,
        //        BranchId = 1
        //    };

        //    // Act
        //    var badRequestResult = _sut.AssignManagerToBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Manager with id {workerAtBranch.WorkerId} is currently assigned to branch with id {_secondBranch.Id}.");
        //}

        //[TestMethod]
        //public void ExpelTellerFromBranch_Should_SetWorkAtIdPropertyToNull_And_FillTellerAtBranchHistoryRecord_And_ReturnOkObjectResult_When_ModelStateIsValid()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 3,
        //        BranchId = 2
        //    };

        //    var currentUser = new ApplicationUser { Id = 3 };
        //    var claims = new List<Claim> { new Claim(CustomClaimTypes.UserId, currentUser.Id.ToString()) };
        //    var identity = new ClaimsIdentity(claims);
        //    var claimsPrincipal = new ClaimsPrincipal(identity);
        //    var context = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = claimsPrincipal
        //        }
        //    };

        //    _sut.ControllerContext = context;

        //    // Act
        //    var okResult = _sut.ExpelTellerFromBranch(workerAtBranch) as OkResult;

        //    // Assert
        //    Assert.IsNotNull(okResult);

        //    var tellerFromDb = _context.Tellers.SingleOrDefault(t => t.Id == workerAtBranch.WorkerId);
        //    Assert.IsNotNull(tellerFromDb);
        //    Assert.AreEqual(null, tellerFromDb.WorkAtId);

        //    var tellerAtBranchFromDb = _context.TellerAtBranchHistory.Where(t => t.TellerId == workerAtBranch.WorkerId).ToList().LastOrDefault();
        //    Assert.IsNotNull(tellerAtBranchFromDb);
        //    Assert.IsNotNull(tellerAtBranchFromDb.ExpelDate);
        //    Assert.AreEqual(_tellerAtBranchHistory.AssignDate, tellerAtBranchFromDb.AssignDate);
        //    Assert.AreEqual(_tellerAtBranchHistory.AssignedById, tellerAtBranchFromDb.AssignedById);
        //    Assert.AreEqual(currentUser.Id, tellerAtBranchFromDb.ExpelledById);
        //    Assert.AreEqual(workerAtBranch.BranchId, tellerAtBranchFromDb.BranchId);
        //    Assert.AreEqual(workerAtBranch.WorkerId, tellerAtBranchFromDb.TellerId);
        //}

        //[TestMethod]
        //public void ExpelTellerFromBranch_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto();

        //    _sut.ModelState.AddModelError(nameof(workerAtBranch.WorkerId), $"The {nameof(workerAtBranch.WorkerId)} field is required.");
        //    _sut.ModelState.AddModelError(nameof(workerAtBranch.BranchId), $"The {nameof(workerAtBranch.BranchId)} field is required.");

        //    // Act
        //    var badRequestResult = _sut.ExpelTellerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.WorkerId)));
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.WorkerId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"The {nameof(workerAtBranch.WorkerId)} field is required.");

        //    var branchIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(branchIdErrorValues);
        //    Assert.IsTrue(branchIdErrorValues.Single() == $"The {nameof(workerAtBranch.BranchId)} field is required.");
        //}

        //[TestMethod]
        //public void ExpelTellerFromBranch_Should_ReturnBadRequest_When_TellerNotExist()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 999,
        //        BranchId = 2
        //    };

        //    // Act
        //    var badRequestResult = _sut.ExpelTellerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.WorkerId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.WorkerId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Teller with id {workerAtBranch.WorkerId} doesn't exist.");
        //}

        //[TestMethod]
        //public void ExpelTellerFromBranch_Should_ReturnBadRequest_When_BranchNotExist()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 3,
        //        BranchId = 999
        //    };

        //    // Act
        //    var badRequestResult = _sut.ExpelTellerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Branch with id {workerAtBranch.BranchId} doesn't exist.");
        //}

        //[TestMethod]
        //public void ExpelTellerFromBranch_Should_ReturnBadRequest_When_TellerIsNotAssignedToBranch()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 2,
        //        BranchId = 2
        //    };

        //    // Act
        //    var badRequestResult = _sut.ExpelTellerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Teller with id {workerAtBranch.WorkerId} is currently not assigned to any branch.");
        //}

        //[TestMethod]
        //public void ExpelTellerFromBranch_Should_ReturnBadRequest_When_TellerIsAssignedToOtherBranch()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 3,
        //        BranchId = 1
        //    };

        //    // Act
        //    var badRequestResult = _sut.ExpelTellerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Teller with id {workerAtBranch.WorkerId} is currently not assigned to branch with id {_secondBranch.Id}.");
        //}

        //[TestMethod]
        //public void ExpelManagerFromBranch_Should_SetWorkAtIdPropertyToNull_And_FillManagerAtBranchHistoryRecord_And_ReturnOkObjectResult_When_ModelStateIsValid()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 5,
        //        BranchId = 2
        //    };

        //    var currentUser = new ApplicationUser { Id = 5 };
        //    var claims = new List<Claim> { new Claim(CustomClaimTypes.UserId, currentUser.Id.ToString()) };
        //    var identity = new ClaimsIdentity(claims);
        //    var claimsPrincipal = new ClaimsPrincipal(identity);
        //    var context = new ControllerContext
        //    {
        //        HttpContext = new DefaultHttpContext
        //        {
        //            User = claimsPrincipal
        //        }
        //    };

        //    _sut.ControllerContext = context;

        //    // Act
        //    var okResult = _sut.ExpelManagerFromBranch(workerAtBranch) as OkResult;

        //    // Assert
        //    Assert.IsNotNull(okResult);

        //    var managerFromDb = _context.Managers.SingleOrDefault(m => m.Id == workerAtBranch.WorkerId);
        //    Assert.IsNotNull(managerFromDb);
        //    Assert.AreEqual(null, managerFromDb.WorkAtId);

        //    var managerAtBranchFromDb = _context.ManagerAtBranchHistory.Where(m => m.ManagerId == workerAtBranch.WorkerId).ToList().LastOrDefault();
        //    Assert.IsNotNull(managerAtBranchFromDb);
        //    Assert.IsNotNull(managerAtBranchFromDb.ExpelDate);
        //    Assert.AreEqual(_managerAtBranchHistory.AssignDate, managerAtBranchFromDb.AssignDate);
        //    Assert.AreEqual(_managerAtBranchHistory.AssignedById, managerAtBranchFromDb.AssignedById);
        //    Assert.AreEqual(currentUser.Id, managerAtBranchFromDb.ExpelledById);
        //    Assert.AreEqual(workerAtBranch.BranchId, managerAtBranchFromDb.BranchId);
        //    Assert.AreEqual(workerAtBranch.WorkerId, managerAtBranchFromDb.ManagerId);
        //}

        //[TestMethod]
        //public void ExpelManagerFromBranch_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto();

        //    _sut.ModelState.AddModelError(nameof(workerAtBranch.WorkerId), $"The {nameof(workerAtBranch.WorkerId)} field is required.");
        //    _sut.ModelState.AddModelError(nameof(workerAtBranch.BranchId), $"The {nameof(workerAtBranch.BranchId)} field is required.");

        //    // Act
        //    var badRequestResult = _sut.ExpelManagerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.WorkerId)));
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.WorkerId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"The {nameof(workerAtBranch.WorkerId)} field is required.");

        //    var branchIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(branchIdErrorValues);
        //    Assert.IsTrue(branchIdErrorValues.Single() == $"The {nameof(workerAtBranch.BranchId)} field is required.");
        //}

        //[TestMethod]
        //public void ExpelManagerFromBranch_Should_ReturnBadRequest_When_ManagerNotExist()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 999,
        //        BranchId = 2
        //    };

        //    // Act
        //    var badRequestResult = _sut.ExpelManagerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.WorkerId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.WorkerId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Manager with id {workerAtBranch.WorkerId} doesn't exist.");
        //}

        //[TestMethod]
        //public void ExpelManagerFromBranch_Should_ReturnBadRequest_When_BranchNotExist()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 5,
        //        BranchId = 999
        //    };

        //    // Act
        //    var badRequestResult = _sut.ExpelManagerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Branch with id {workerAtBranch.BranchId} doesn't exist.");
        //}

        //[TestMethod]
        //public void ExpelManagerFromBranch_Should_ReturnBadRequest_When_ManagerIsNotAssignedToBranch()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 4,
        //        BranchId = 2
        //    };

        //    // Act
        //    var badRequestResult = _sut.ExpelManagerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Manager with id {workerAtBranch.WorkerId} is currently not assigned to any branch.");
        //}

        //[TestMethod]
        //public void ExpelManagerFromBranch_Should_ReturnBadRequest_When_ManagerIsAssignedToOtherBranch()
        //{
        //    // Arrange
        //    var workerAtBranch = new WorkerAtBranchDto
        //    {
        //        WorkerId = 5,
        //        BranchId = 1
        //    };

        //    // Act
        //    var badRequestResult = _sut.ExpelManagerFromBranch(workerAtBranch) as BadRequestObjectResult;

        //    // Assert
        //    Assert.IsNotNull(badRequestResult);
        //    Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));

        //    var error = badRequestResult.Value as SerializableError;
        //    Assert.IsNotNull(error);
        //    Assert.IsTrue(error.ContainsKey(nameof(workerAtBranch.BranchId)));

        //    var workerIdErrorValues = error[nameof(workerAtBranch.BranchId)] as string[];
        //    Assert.IsNotNull(workerIdErrorValues);
        //    Assert.IsTrue(workerIdErrorValues.Single() == $"Manager with id {workerAtBranch.WorkerId} is currently not assigned to branch with id {_secondBranch.Id}.");
        //}
    }
}