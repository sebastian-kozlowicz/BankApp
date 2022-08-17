using System.Collections.Generic;
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

            _sut.ModelState.AddModelError(nameof(workerAtBranch.WorkerId),
                $"The {nameof(workerAtBranch.WorkerId)} field is required.");
            _sut.ModelState.AddModelError(nameof(workerAtBranch.BranchId),
                $"The {nameof(workerAtBranch.BranchId)} field is required.");

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

        [TestMethod]
        public async Task AssignManagerToBranchAsync_Should_ReturnOkObjectResult_When_ModelStateIsValid()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 4,
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
            var result = await _sut.AssignManagerToBranchAsync(workerAtBranch);

            // Assert
            var createdAtRouteResult = result as OkResult;
            createdAtRouteResult.Should().NotBeNull();

            createdAtRouteResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task AssignManagerToBranchAsync_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto();

            _sut.ModelState.AddModelError(nameof(workerAtBranch.WorkerId),
                $"The {nameof(workerAtBranch.WorkerId)} field is required.");
            _sut.ModelState.AddModelError(nameof(workerAtBranch.BranchId),
                $"The {nameof(workerAtBranch.BranchId)} field is required.");

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
            var result = await _sut.AssignManagerToBranchAsync(workerAtBranch);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task ExpelTellerFromBranch_Should_ReturnOkObjectResult_When_ModelStateIsValid()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 3,
                BranchId = 2
            };

            var currentUser = new ApplicationUser { Id = 3 };
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
            var result = await _sut.ExpelTellerFromBranchAsync(workerAtBranch);

            // Assert
            var createdAtRouteResult = result as OkResult;
            createdAtRouteResult.Should().NotBeNull();

            createdAtRouteResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task ExpelTellerFromBranch_Should_ReturnBadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto();

            _sut.ModelState.AddModelError(nameof(workerAtBranch.WorkerId),
                $"The {nameof(workerAtBranch.WorkerId)} field is required.");
            _sut.ModelState.AddModelError(nameof(workerAtBranch.BranchId),
                $"The {nameof(workerAtBranch.BranchId)} field is required.");

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
            var result = await _sut.ExpelTellerFromBranchAsync(workerAtBranch);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task ExpelManagerFromBranchAsync_Should_ReturnOkObjectResult_When_ModelStateIsValid()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 5,
                BranchId = 2
            };

            var currentUser = new ApplicationUser { Id = 5 };
            var claims = new List<Claim> { new Claim(CustomClaimTypes.UserId, currentUser.Id.ToString()) };
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
            var result = await _sut.ExpelManagerFromBranchAsync(workerAtBranch);

            // Assert
            var createdAtRouteResult = result as OkResult;
            createdAtRouteResult.Should().NotBeNull();

            createdAtRouteResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task ExpelManagerFromBranch_Should_ReturnBadRequest_When_ModelStateIsInvalid()
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
            var result = await _sut.ExpelManagerFromBranchAsync(workerAtBranch);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();

            var serializableError = badRequestResult.Value as SerializableError;
            serializableError.Should().BeEquivalentTo(expectedResult);
        }
    }
}