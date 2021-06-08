using System;
using System.Collections.Generic;
using BankApp.Configuration;
using BankApp.Helpers.Builders.Logging;
using BankApp.Models.RequestResponseLogging;
using BankApp.UnitTests.Helpers.Builders.HelperModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BankApp.UnitTests.Helpers.Builders
{
    [TestClass]
    public class RequestResponseLoggingBuilderTests
    {
        private readonly Mock<IOptions<LogSanitizationOptions>> _logSanitizationOptionsMock = new();
        private RequestResponseLoggingBuilder _sut;

        [TestMethod]
        public void
            GenerateRequestLogMessage_Should_ReturnExpectedNotSanitizedLogMessage_When_LogSanitizationIsDisabled()
        {
            // Arrange
            var requestInfo = new RequestInfo
            {
                TraceIdentifier = "80000006-0000-fc00-b63f-84710c7967bb",
                Headers = new HeaderDictionary
                {
                    {
                        "Host", new StringValues("{localhost:44387}")
                    },
                    {
                        "Authentication", new StringValues("{Bearer qwerty}")
                    }
                },
                ActionArguments = new Dictionary<string, object>
                {
                    {"queryParamIds", new List<int> {1, 2, 3}},
                    {
                        "inputObject", new ParentModel
                        {
                            ParentInt = 1,
                            ParentString = "String",
                            ParentDateTime = new DateTime(2021, 1, 1),
                            ChildModel = new ChildModel
                            {
                                ChildInt = 2,
                                ChildString = "String2",
                                ChildDateTime = new DateTime(2022, 12, 31),
                                ComplexModel = new ComplexModel
                                {
                                    ComplexInt = 3,
                                    ComplexString = "String3",
                                    ComplexDateTime = new DateTime(2023, 1, 1)
                                },
                                ComplexModelSanitized = new ComplexModel
                                {
                                    ComplexInt = 4,
                                    ComplexString = "String4",
                                    ComplexDateTime = new DateTime(2023, 12, 31)
                                }
                            },
                            ChildListModel = new List<ChildListModel>
                            {
                                new()
                                {
                                    ChildListInt = 5,
                                    ChildListString = "String5",
                                    ChildListDateTime = new DateTime(2024, 1, 1),
                                    ComplexListModel = new ComplexListModel
                                    {
                                        ComplexListInt = 6,
                                        ComplexListString = "String6",
                                        ComplexListDateTime = new DateTime(2024, 12, 31)
                                    },
                                    ComplexListModelSanitized = new ComplexListModel
                                    {
                                        ComplexListInt = 7,
                                        ComplexListString = "String7",
                                        ComplexListDateTime = new DateTime(2025, 1, 1)
                                    }
                                }
                            }
                        }
                    }
                },
                Method = "POST",
                Path = "/api/unit-test"
            };

            _logSanitizationOptionsMock.Setup(o => o.Value).Returns(new LogSanitizationOptions
            {
                IsEnabled = false
            });

            _sut = new RequestResponseLoggingBuilder(_logSanitizationOptionsMock.Object, new LogSanitizedBuilder(),
                new SensitiveDataPropertyNamesBuilder());

            // Act
            var result = _sut.GenerateRequestLogMessage(requestInfo);

            // Assert
            result.Should()
                .Be(
                    "Http Request Information: \r\nTrace Identifier: 80000006-0000-fc00-b63f-84710c7967bb \r\nMethod: POST \r\nScheme:  \r\nPath: /api/unit-test \r\nHeaders: \r\nHost: {localhost:44387}\r\nAuthentication: {Bearer qwerty} \r\nAction Arguments: \r\nqueryParamIds: [1,2,3]\r\ninputObject: {\"ParentInt\":1,\"ParentString\":\"String\",\"ParentDateTime\":\"2021-01-01T00:00:00\",\"ChildModel\":{\"ChildInt\":2,\"ChildString\":\"String2\",\"ChildDateTime\":\"2022-12-31T00:00:00\",\"ComplexModel\":{\"ComplexInt\":3,\"ComplexString\":\"String3\",\"ComplexDateTime\":\"2023-01-01T00:00:00\"},\"ComplexModelSanitized\":{\"ComplexInt\":4,\"ComplexString\":\"String4\",\"ComplexDateTime\":\"2023-12-31T00:00:00\"}},\"ChildListModel\":[{\"ChildListInt\":5,\"ChildListString\":\"String5\",\"ChildListDateTime\":\"2024-01-01T00:00:00\",\"ComplexListModel\":{\"ComplexListInt\":6,\"ComplexListString\":\"String6\",\"ComplexListDateTime\":\"2024-12-31T00:00:00\"},\"ComplexListModelSanitized\":{\"ComplexListInt\":7,\"ComplexListString\":\"String7\",\"ComplexListDateTime\":\"2025-01-01T00:00:00\"}}]}");
        }
    }
}