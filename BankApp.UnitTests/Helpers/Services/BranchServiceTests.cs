using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Address;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Exceptions;
using BankApp.Helpers.Services;
using BankApp.Mapping;
using BankApp.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankApp.UnitTests.Helpers.Services
{
    [TestClass]
    public class BranchServiceTests
    {
        private readonly Branch _firstBranch = new()
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

        private readonly ManagerAtBranchHistory _managerAtBranchHistory = new()
        {
            Id = 1,
            AssignDate = new DateTime(2021, 1, 1),
            BranchId = 2,
            ManagerId = 5,
            AssignedById = 1
        };

        private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();

        private readonly Branch _secondBranch = new()
        {
            Id = 2,
            BranchCode = "001",
            BranchAddress = new BranchAddress
            {
                Id = 2,
                Country = "Poland",
                City = "Poznań",
                Street = "Gwarna",
                HouseNumber = "2",
                ApartmentNumber = "4",
                PostalCode = "61-703"
            }
        };

        private readonly TellerAtBranchHistory _tellerAtBranchHistory = new()
        {
            Id = 1,
            AssignDate = new DateTime(2021, 1, 1),
            BranchId = 2,
            TellerId = 3,
            AssignedById = 1
        };

        private ApplicationDbContext _context;
        private BranchService _sut;
        private IEnumerable<Branch> Branches => new List<Branch> { _firstBranch, _secondBranch };

        private ApplicationDbContext GetMockContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.Branches.AddRange(Branches);
            context.Users.Add(new ApplicationUser { Id = 1, Administrator = new Administrator { Id = 1 } });
            context.Users.Add(new ApplicationUser { Id = 2, Teller = new Teller { Id = 2 } });
            context.Users.Add(new ApplicationUser { Id = 3, Teller = new Teller { Id = 3, WorkAtId = 2 } });
            context.Users.Add(new ApplicationUser { Id = 4, Manager = new Manager { Id = 4 } });
            context.Users.Add(new ApplicationUser { Id = 5, Manager = new Manager { Id = 5, WorkAtId = 2 } });
            context.TellerAtBranchHistory.Add(_tellerAtBranchHistory);
            context.ManagerAtBranchHistory.Add(_managerAtBranchHistory);

            context.SaveChanges();

            return context;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = GetMockContext();
            _sut = new BranchService(_context, _mapper);
        }

        [TestMethod]
        public async Task GetBranchAsync_Should_ReturnBranch_When_BranchIsFound()
        {
            var result = await _sut.GetBranchAsync(1);

            result.Should().Be(_firstBranch);
        }

        [TestMethod]
        public async Task GetBranchAsync_Should_ReturnNull_When_BranchIsNotFound()
        {
            var result = await _sut.GetBranchAsync(999);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateBranchWithAddressAsync_Should_CreateBranchWithAddress_And_ReturnBranch()
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

            var expectedBranch = new Branch
            {
                Id = 3,
                BranchCode = branchCreation.Branch.BranchCode,
                BranchAddress = new BranchAddress
                {
                    Id = 3,
                    Country = branchCreation.Address.Country,
                    City = branchCreation.Address.City,
                    Street = branchCreation.Address.Street,
                    HouseNumber = branchCreation.Address.HouseNumber,
                    ApartmentNumber = branchCreation.Address.ApartmentNumber,
                    PostalCode = branchCreation.Address.PostalCode
                }
            };

            // Act
            var result = await _sut.CreateBranchWithAddressAsync(branchCreation);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedBranch.Id);
            result.BranchCode.Should().Be(expectedBranch.BranchCode);
            result.BranchAddress.Id.Should().Be(expectedBranch.BranchAddress.Id);
            result.BranchAddress.Country.Should().Be(expectedBranch.BranchAddress.Country);
            result.BranchAddress.City.Should().Be(expectedBranch.BranchAddress.City);
            result.BranchAddress.Street.Should().Be(expectedBranch.BranchAddress.Street);
            result.BranchAddress.HouseNumber.Should().Be(expectedBranch.BranchAddress.HouseNumber);
            result.BranchAddress.ApartmentNumber.Should().Be(expectedBranch.BranchAddress.ApartmentNumber);
            result.BranchAddress.PostalCode.Should().Be(expectedBranch.BranchAddress.PostalCode);

            var branchFromDb = _context.Branches.SingleOrDefault(b => b.Id == result.Id);
            branchFromDb.Should().NotBeNull();
            branchFromDb.Id.Should().Be(expectedBranch.Id);
            branchFromDb.BranchCode.Should().Be(expectedBranch.BranchCode);
            branchFromDb.BranchAddress.Id.Should().Be(expectedBranch.BranchAddress.Id);
            branchFromDb.BranchAddress.Country.Should().Be(expectedBranch.BranchAddress.Country);
            branchFromDb.BranchAddress.City.Should().Be(expectedBranch.BranchAddress.City);
            branchFromDb.BranchAddress.Street.Should().Be(expectedBranch.BranchAddress.Street);
            branchFromDb.BranchAddress.HouseNumber.Should().Be(expectedBranch.BranchAddress.HouseNumber);
            branchFromDb.BranchAddress.ApartmentNumber.Should().Be(expectedBranch.BranchAddress.ApartmentNumber);
            branchFromDb.BranchAddress.PostalCode.Should().Be(expectedBranch.BranchAddress.PostalCode);
        }

        [TestMethod]
        public void CreateBranchWithAddressAsync_Should_ThrowValidationException_When_BranchCodeIsAlreadyInUse()
        {
            // Arrange
            var branchCreation = new BranchWithAddressCreationDto
            {
                Branch = new BranchCreationDto
                {
                    BranchCode = "000"
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
            Func<Task> func = async () => await _sut.CreateBranchWithAddressAsync(branchCreation);

            // Assert
            func.Should().Throw<ValidationException>().WithMessage("Branch code is already in use.");
        }

        [TestMethod]
        public async Task
            AssignTellerToBranchAsync_Should_SetWorkAtIdPropertyToSuppliedBranchId_And_CreateTellerAtBranchHistory_And_ReturnTrue()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 2,
                BranchId = 1
            };

            const int currentUserId = 1;

            // Act
            var result = await _sut.AssignTellerToBranchAsync(workerAtBranch, currentUserId);

            // Assert
            result.Should().BeTrue();

            var tellerFromDb = _context.Tellers.SingleOrDefault(t => t.Id == workerAtBranch.WorkerId);
            tellerFromDb.Should().NotBeNull();
            tellerFromDb.WorkAtId.Should().Be(workerAtBranch.BranchId);

            var tellerAtBranchFromDb = _context.TellerAtBranchHistory.Where(t => t.TellerId == workerAtBranch.WorkerId)
                .ToList().LastOrDefault();
            tellerAtBranchFromDb.Should().NotBeNull();
            tellerAtBranchFromDb.AssignDate.Should().NotBe(DateTime.MinValue);
            tellerAtBranchFromDb.AssignedById.Should().Be(currentUserId);
            tellerAtBranchFromDb.ExpelDate.Should().BeNull();
            tellerAtBranchFromDb.ExpelledById.Should().BeNull();
            tellerAtBranchFromDb.ExpelledBy.Should().BeNull();
            tellerAtBranchFromDb.BranchId.Should().Be(workerAtBranch.BranchId);
            tellerAtBranchFromDb.TellerId.Should().Be(workerAtBranch.WorkerId);
        }

        [TestMethod]
        public void AssignTellerToBranchAsync_Should_ThrowValidationException_When_TellerNotExist()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 999,
                BranchId = 1
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.AssignTellerToBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>()
                .WithMessage($"Teller with id {workerAtBranch.WorkerId} doesn't exist.");
        }

        [TestMethod]
        public void AssignTellerToBranchAsync_Should_ThrowValidationException_When_BranchNotExist()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 2,
                BranchId = 999
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.AssignTellerToBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>()
                .WithMessage($"Branch with id {workerAtBranch.BranchId} doesn't exist.");
        }

        [TestMethod]
        public void AssignTellerToBranchAsync_Should_ThrowValidationException_When_TellerIsAlreadyAssignedToBranch()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 3,
                BranchId = 1
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.AssignTellerToBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>().WithMessage(
                $"Teller with id {workerAtBranch.WorkerId} is currently assigned to branch with id {_secondBranch.Id}.");
        }

        [TestMethod]
        public async Task
            AssignManagerToBranchAsync_Should_SetWorkAtIdPropertyToSuppliedBranchId_And_CreateManagerAtBranchHistory_And_ReturnTrue()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 4,
                BranchId = 1
            };

            const int currentUserId = 1;

            // Act
            var result = await _sut.AssignManagerToBranchAsync(workerAtBranch, currentUserId);

            // Assert
            result.Should().BeTrue();

            var managerFromDb = _context.Managers.SingleOrDefault(t => t.Id == workerAtBranch.WorkerId);
            managerFromDb.Should().NotBeNull();
            managerFromDb.WorkAtId.Should().Be(workerAtBranch.BranchId);

            var managerAtBranchFromDb = _context.ManagerAtBranchHistory
                .Where(t => t.ManagerId == workerAtBranch.WorkerId)
                .ToList().LastOrDefault();
            managerAtBranchFromDb.Should().NotBeNull();
            managerAtBranchFromDb.AssignDate.Should().NotBe(DateTime.MinValue);
            managerAtBranchFromDb.AssignedById.Should().Be(currentUserId);
            managerAtBranchFromDb.ExpelDate.Should().BeNull();
            managerAtBranchFromDb.ExpelledById.Should().BeNull();
            managerAtBranchFromDb.ExpelledBy.Should().BeNull();
            managerAtBranchFromDb.BranchId.Should().Be(workerAtBranch.BranchId);
            managerAtBranchFromDb.ManagerId.Should().Be(workerAtBranch.WorkerId);
        }

        [TestMethod]
        public void AssignManagerToBranchAsync_Should_ThrowValidationException_When_ManagerNotExist()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 999,
                BranchId = 1
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.AssignManagerToBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>()
                .WithMessage($"Manager with id {workerAtBranch.WorkerId} doesn't exist.");
        }

        [TestMethod]
        public void AssignManagerToBranchAsync_Should_ThrowValidationException_When_BranchNotExist()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 4,
                BranchId = 999
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.AssignManagerToBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>()
                .WithMessage($"Branch with id {workerAtBranch.BranchId} doesn't exist.");
        }

        [TestMethod]
        public void AssignManagerToBranchAsync_Should_ThrowValidationException_When_ManagerIsAlreadyAssignedToBranch()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 5,
                BranchId = 1
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.AssignManagerToBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>().WithMessage(
                $"Manager with id {workerAtBranch.WorkerId} is currently assigned to branch with id {_secondBranch.Id}.");
        }

        [TestMethod]
        public async Task
            ExpelTellerFromBranchAsync_Should_SetWorkAtIdPropertyToNull_And_FillTellerAtBranchHistoryRecord_And_ReturnTrue()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 3,
                BranchId = 2
            };

            const int currentUserId = 1;

            // Act
            var result = await _sut.ExpelTellerFromBranchAsync(workerAtBranch, currentUserId);

            // Assert
            result.Should().BeTrue();

            var tellerFromDb = _context.Tellers.SingleOrDefault(t => t.Id == workerAtBranch.WorkerId);
            tellerFromDb.Should().NotBeNull();
            tellerFromDb.WorkAtId.Should().BeNull();

            var tellerAtBranchFromDb = _context.TellerAtBranchHistory.Where(t => t.TellerId == workerAtBranch.WorkerId)
                .ToList().LastOrDefault();
            tellerAtBranchFromDb.Should().NotBeNull();
            tellerAtBranchFromDb.AssignDate.Should().Be(_tellerAtBranchHistory.AssignDate);
            tellerAtBranchFromDb.AssignedById.Should().Be(currentUserId);
            tellerAtBranchFromDb.ExpelDate.Should().NotBeNull();
            tellerAtBranchFromDb.ExpelDate.Should().NotBe(DateTime.MinValue);
            tellerAtBranchFromDb.ExpelledById.Should().Be(currentUserId);
            tellerAtBranchFromDb.ExpelledBy.Should().NotBeNull();
            tellerAtBranchFromDb.BranchId.Should().Be(workerAtBranch.BranchId);
            tellerAtBranchFromDb.TellerId.Should().Be(workerAtBranch.WorkerId);
        }

        [TestMethod]
        public void ExpelTellerFromBranchAsync_Should_ThrowValidationException_When_TellerNotExist()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 999,
                BranchId = 2
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.ExpelTellerFromBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>().WithMessage(
                $"Teller with id {workerAtBranch.WorkerId} doesn't exist.");
        }

        [TestMethod]
        public void ExpelTellerFromBranchAsync_Should_ThrowValidationException_WhenBranchNotExist()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 3,
                BranchId = 999
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.ExpelTellerFromBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>().WithMessage(
                $"Branch with id {workerAtBranch.BranchId} doesn't exist.");
        }

        [TestMethod]
        public void ExpelTellerFromBranchAsync_Should_ThrowValidationException_TellerIsNotAssignedToBranch()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 2,
                BranchId = 2
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.ExpelTellerFromBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>().WithMessage(
                $"Teller with id {workerAtBranch.WorkerId} is currently not assigned to any branch.");
        }
        
        [TestMethod]
        public void ExpelTellerFromBranchAsync_Should_ThrowValidationException_TellerIsAssignedToOtherBranch()
        {
            // Arrange
            var workerAtBranch = new WorkerAtBranchDto
            {
                WorkerId = 3,
                BranchId = 1
            };

            const int currentUserId = 1;

            // Act
            Func<Task> func = async () => await _sut.ExpelTellerFromBranchAsync(workerAtBranch, currentUserId);

            // Assert
            func.Should().Throw<ValidationException>().WithMessage(
                $"Teller with id {workerAtBranch.WorkerId} is currently not assigned to branch with id {_secondBranch.Id}.");
        }
    }
}