using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using BankApp.Mapping;
using BankApp.Models;
using BankApp.Dtos.BankAccount;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankApp.UnitTests.Migration
{
    [TestClass]
    public class MappingRegressionTests
    {
        [TestMethod]
        public void EveryProductionMapHasBoundedRecursion()
        {
            var configuration = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            var maps = configuration.Internal().GetAllTypeMaps().ToArray();
            Assert.IsTrue(maps.Length >= 13);
            foreach (var map in maps)
                Assert.IsTrue(map.MaxDepth > 0 && map.MaxDepth <= 64,
                    $"Unbounded map: {map.SourceType.Name} -> {map.DestinationType.Name}");
        }

        [TestMethod]
        public void DepthGuardStopsDeepObjectGraphWithoutLosingShallowValues()
        {
            var mapper = new MapperConfiguration(c => c.AddProfile<RecursiveProfile>()).CreateMapper();
            var source = new SourceNode { Value = 1 };
            var tail = source;
            for (var i = 2; i <= 1000; i++)
            {
                tail.Next = new SourceNode { Value = i };
                tail = tail.Next;
            }
            var result = mapper.Map<DestinationNode>(source);
            var count = 0;
            for (var node = result; node != null; node = node.Next)
            {
                count++;
                Assert.AreEqual(count, node.Value);
                Assert.IsTrue(count <= 64, "Depth guard did not truncate the graph.");
            }
            Assert.AreEqual(64, count);
        }

        public class SourceNode
        {
            public int Value { get; set; }
            public SourceNode Next { get; set; }
        }

        public class DestinationNode
        {
            public int Value { get; set; }
            public DestinationNode Next { get; set; }
        }

        private class RecursiveProfile : MappingProfile
        {
            public RecursiveProfile() => CreateMap<SourceNode, DestinationNode>();
        }

        [TestMethod]
        public void AccountMappingPreservesMoneyIdentifiersAndNestedCustomer()
        {
            var mapper = new MapperConfiguration(c => c.AddProfile<MappingProfile>()).CreateMapper();
            var account = new BankAccount
            {
                Id = 17, Iban = "PL61108000010000000000000000", Balance = 123.45m,
                DebitLimit = 987.65m, CustomerId = 7, CreatedById = 8,
                Customer = new Customer { Id = 7, ApplicationUser = new ApplicationUser { Id = 7, Name = "Jan" } },
                CreatedBy = new ApplicationUser { Id = 8, Name = "Anna" }
            };
            var dto = mapper.Map<BankAccountDto>(account);
            Assert.AreEqual(account.Id, dto.Id);
            Assert.AreEqual(account.Iban, dto.Iban);
            Assert.AreEqual(account.Balance, dto.Balance);
            Assert.AreEqual(account.DebitLimit, dto.DebitLimit);
            Assert.AreEqual(7, dto.Customer.Id);
            Assert.AreEqual("Anna", dto.CreatedBy.Name);
        }
    }
}
