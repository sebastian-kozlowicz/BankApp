using System;
using System.Collections.Generic;
using BankApp.Configuration;
using BankApp.Helpers.Builders.Logging;
using BankApp.Models.RequestResponseLogging;
using BankApp.UnitTests.Helpers.Builders.HelperModels.ListModel;
using BankApp.UnitTests.Helpers.Builders.HelperModels.ObjectModel;
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
                    {"queryParamIntegers", new List<int> {1, 2, 3}},
                    {"queryParamStrings", new List<string> {"one", "two", "three"}},
                    {
                        "inputObject", new ParentObjectModel
                        {
                            ParentObjectInt = 1,
                            ParentObjectString = "String",
                            ParentObjectDateTime = new DateTime(2021, 1, 1),
                            ChildObjectModel = new ChildObjectModel
                            {
                                ChildObjectInt = 2,
                                ChildObjectString = "String2",
                                ChildObjectDateTime = new DateTime(2022, 12, 31),
                                ComplexObjectModel = new ComplexObjectModel
                                {
                                    ComplexObjectInt = 3,
                                    ComplexObjectString = "String3",
                                    ComplexObjectDateTime = new DateTime(2023, 1, 1)
                                },
                                ComplexObjectModelSanitized = new ComplexObjectModel
                                {
                                    ComplexObjectInt = 4,
                                    ComplexObjectString = "String4",
                                    ComplexObjectDateTime = new DateTime(2023, 12, 31)
                                }
                            },
                            ChildrenObjectModel = new List<ChildrenObjectModel>
                            {
                                new()
                                {
                                    ChildrenObjectInt = 5,
                                    ChildrenObjectString = "String5",
                                    ChildrenObjectDateTime = new DateTime(2024, 1, 1),
                                    ComplexChildrenObjectModel = new ComplexChildrenObjectModel
                                    {
                                        ComplexChildrenObjectInt = 6,
                                        ComplexChildrenObjectString = "String6",
                                        ComplexChildrenObjectDateTime = new DateTime(2024, 12, 31)
                                    },
                                    ComplexChildrenObjectModelSanitized = new ComplexChildrenObjectModel
                                    {
                                        ComplexChildrenObjectInt = 7,
                                        ComplexChildrenObjectString = "String7",
                                        ComplexChildrenObjectDateTime = new DateTime(2025, 1, 1)
                                    }
                                }
                            }
                        }
                    },
                    {
                        "inputList", new List<ParentListModel>
                        {
                            new()
                            {
                                ParentListInt = 1,
                                ParentListString = "String",
                                ParentListDateTime = new DateTime(2021, 1, 1),
                                ChildListModel = new ChildListModel
                                {
                                    ChildListInt = 2,
                                    ChildListString = "String2",
                                    ChildListDateTime = new DateTime(2022, 12, 31),
                                    ComplexListModel = new ComplexListModel
                                    {
                                        ComplexListInt = 3,
                                        ComplexListString = "String3",
                                        ComplexListDateTime = new DateTime(2023, 1, 1)
                                    },
                                    ComplexListModelSanitized = new ComplexListModel
                                    {
                                        ComplexListInt = 4,
                                        ComplexListString = "String4",
                                        ComplexListDateTime = new DateTime(2023, 12, 31)
                                    }
                                },
                                ChildrenListModel = new List<ChildrenListModel>
                                {
                                    new()
                                    {
                                        ChildrenListInt = 5,
                                        ChildrenListString = "String5",
                                        ChildrenListDateTime = new DateTime(2024, 1, 1),
                                        ComplexChildrenListModel = new ComplexChildrenListModel
                                        {
                                            ComplexChildrenListInt = 6,
                                            ComplexChildrenListString = "String6",
                                            ComplexChildrenListDateTime = new DateTime(2024, 12, 31)
                                        },
                                        ComplexChildrenListModelSanitized = new ComplexChildrenListModel
                                        {
                                            ComplexChildrenListInt = 7,
                                            ComplexChildrenListString = "String7",
                                            ComplexChildrenListDateTime = new DateTime(2025, 1, 1)
                                        }
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
                    "Http Request Information: \r\nTrace Identifier: 80000006-0000-fc00-b63f-84710c7967bb \r\nMethod: POST \r\nScheme:  \r\nPath: /api/unit-test \r\nHeaders: \r\nHost: {localhost:44387}\r\nAuthentication: {Bearer qwerty} \r\nAction Arguments: \r\nqueryParamIntegers: [1,2,3]\r\nqueryParamStrings: [\"one\",\"two\",\"three\"]\r\ninputObject: {\"ParentObjectInt\":1,\"ParentObjectString\":\"String\",\"ParentObjectDateTime\":\"2021-01-01T00:00:00\",\"ChildObjectModel\":{\"ChildObjectInt\":2,\"ChildObjectString\":\"String2\",\"ChildObjectDateTime\":\"2022-12-31T00:00:00\",\"ComplexObjectModel\":{\"ComplexObjectInt\":3,\"ComplexObjectString\":\"String3\",\"ComplexObjectDateTime\":\"2023-01-01T00:00:00\"},\"ComplexObjectModelSanitized\":{\"ComplexObjectInt\":4,\"ComplexObjectString\":\"String4\",\"ComplexObjectDateTime\":\"2023-12-31T00:00:00\"}},\"ChildrenObjectModel\":[{\"ChildrenObjectInt\":5,\"ChildrenObjectString\":\"String5\",\"ChildrenObjectDateTime\":\"2024-01-01T00:00:00\",\"ComplexChildrenObjectModel\":{\"ComplexChildrenObjectInt\":6,\"ComplexChildrenObjectString\":\"String6\",\"ComplexChildrenObjectDateTime\":\"2024-12-31T00:00:00\"},\"ComplexChildrenObjectModelSanitized\":{\"ComplexChildrenObjectInt\":7,\"ComplexChildrenObjectString\":\"String7\",\"ComplexChildrenObjectDateTime\":\"2025-01-01T00:00:00\"}}]}\r\ninputList: [{\"ParentListInt\":1,\"ParentListString\":\"String\",\"ParentListDateTime\":\"2021-01-01T00:00:00\",\"ChildListModel\":{\"ChildListInt\":2,\"ChildListString\":\"String2\",\"ChildListDateTime\":\"2022-12-31T00:00:00\",\"ComplexListModel\":{\"ComplexListInt\":3,\"ComplexListString\":\"String3\",\"ComplexListDateTime\":\"2023-01-01T00:00:00\"},\"ComplexListModelSanitized\":{\"ComplexListInt\":4,\"ComplexListString\":\"String4\",\"ComplexListDateTime\":\"2023-12-31T00:00:00\"}},\"ChildrenListModel\":[{\"ChildrenListInt\":5,\"ChildrenListString\":\"String5\",\"ChildrenListDateTime\":\"2024-01-01T00:00:00\",\"ComplexChildrenListModel\":{\"ComplexChildrenListInt\":6,\"ComplexChildrenListString\":\"String6\",\"ComplexChildrenListDateTime\":\"2024-12-31T00:00:00\"},\"ComplexChildrenListModelSanitized\":{\"ComplexChildrenListInt\":7,\"ComplexChildrenListString\":\"String7\",\"ComplexChildrenListDateTime\":\"2025-01-01T00:00:00\"}}]}]");
        }
    }
}