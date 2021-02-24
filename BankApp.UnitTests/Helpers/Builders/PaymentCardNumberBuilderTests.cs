using BankApp.Helpers.Builders;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankApp.UnitTests.Helpers.Builders
{
    [TestClass]
    public class PaymentCardNumberBuilderTests
    {
        [TestMethod]
        public void GenerateCheckDigit_Should_Return_ValidCheckDigit()
        {
            // Arrange
            var number = "7992739871";

            // Act
            var result = PaymentCardNumberBuilder.GenerateCheckDigit(number);

            // Assert
            result.Should().Be(3);
        }
    }
}
