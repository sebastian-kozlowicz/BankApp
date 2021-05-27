using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BankApp.Configuration;
using BankApp.Interfaces.Builders;
using BankApp.Models.RequestResponseLogging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankApp.Helpers.Builders
{
    public class RequestResponseLoggingBuilder : IRequestResponseLoggingBuilder
    {
        private readonly List<string> _headersNamesToSanitize = new() {"Authentication"};
        private readonly LogSanitizationOptions _logSanitizationOptions;
        private readonly ILogSanitizedBuilder _logSanitizedBuilder;
        private readonly List<string> _propertyNamesToSanitize = new() {"email", "login", "password"};

        public RequestResponseLoggingBuilder(IOptions<LogSanitizationOptions> logSanitizationOptions,
            ILogSanitizedBuilder logSanitizedBuilder)
        {
            _logSanitizedBuilder = logSanitizedBuilder;
            _logSanitizationOptions = logSanitizationOptions.Value;
        }

        public string GenerateRequestLogMessage(RequestInfo requestInfo)
        {
            var headersAsString = new List<string>();
            var actionArgumentsAsString = new List<string>();

            if (_logSanitizationOptions.IsEnabled)
            {
                var sanitizedHeaders =
                    _logSanitizedBuilder.SanitizeHeaders(requestInfo.Headers, _headersNamesToSanitize);
                headersAsString = sanitizedHeaders;
            }
            else
            {
                foreach (var (key, value) in requestInfo.Headers)
                    headersAsString.Add($"{key}: {value}");
            }

            if (_logSanitizationOptions.IsEnabled)
                foreach (var (key, value) in requestInfo.ActionArguments)
                {
                    var sanitizedPayload = _logSanitizedBuilder.SanitizePayload(JToken.FromObject(value),
                        _propertyNamesToSanitize);
                    actionArgumentsAsString.Add($"{key}: {sanitizedPayload}");
                }
            else
                foreach (var (key, value) in requestInfo.ActionArguments)
                {
                    var payload = JToken.FromObject(value).ToString(Formatting.None);
                    actionArgumentsAsString.Add($"{key}: {payload}");
                }

            return $"Http Request Information: {Environment.NewLine}" +
                   $"Trace Identifier: {requestInfo.TraceIdentifier} {Environment.NewLine}" +
                   $"Method: {requestInfo.Method} {Environment.NewLine}" +
                   $"Path: {requestInfo.Path} {Environment.NewLine}" +
                   $"Headers: {Environment.NewLine}{string.Join($"{Environment.NewLine}", headersAsString)} {Environment.NewLine}" +
                   $"Action Arguments: {Environment.NewLine}{string.Join($"{Environment.NewLine}", actionArgumentsAsString)}";
        }

        public string GenerateResponseLogMessage(ResponseInfo responseInfo)
        {
            var headersAsString = new List<string>();

            if (_logSanitizationOptions.IsEnabled)
            {
                var sanitizedHeaders =
                    _logSanitizedBuilder.SanitizeHeaders(responseInfo.Headers, _headersNamesToSanitize);
                headersAsString = sanitizedHeaders;
            }
            else
            {
                foreach (var (key, value) in responseInfo.Headers)
                    headersAsString.Add($"{key}: {value}");
            }

            var result = responseInfo.IsServerErrorStatusCode
                ? responseInfo.ExceptionMessage
                : _logSanitizedBuilder.SanitizePayload(JToken.FromObject(responseInfo.Result),
                    _propertyNamesToSanitize);

            var responseStringBuilder = new StringBuilder();
            responseStringBuilder.Append($"Http Response Information: {Environment.NewLine}" +
                                         $"Trace Identifier: {responseInfo.TraceIdentifier} {Environment.NewLine}" +
                                         $"Path: {responseInfo.Path} {Environment.NewLine}" +
                                         $"Status Code: {responseInfo.StatusCode} {Environment.NewLine}");

            if (headersAsString.Any())
                responseStringBuilder.Append(
                    $"Headers: {Environment.NewLine}{string.Join($"{Environment.NewLine}", headersAsString)} {Environment.NewLine}");

            responseStringBuilder.Append($"Response: {Environment.NewLine}{result}");

            return responseStringBuilder.ToString();
        }
    }
}