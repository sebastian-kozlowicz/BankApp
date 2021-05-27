using System;
using System.Collections.Generic;
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
        private readonly List<string> _headersNamesToSanitize = new() { "Authentication" };
        private readonly LogSanitizationOptions _logSanitizationOptions;
        private readonly ILogSanitizedBuilder _logSanitizedBuilder;
        private readonly List<string> _propertyNamesToSanitize = new() { "email", "login", "password" };

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
                    var payload = JToken.FromObject(value);
                    actionArgumentsAsString.Add($"{key}: {payload.ToString(Formatting.None)}");
                }

            return $"Http Request Information: {Environment.NewLine}" +
                   $"Method: {requestInfo.Method} {Environment.NewLine}" +
                   $"Path: {requestInfo.Path} {Environment.NewLine}" +
                   $"Trace Identifier: {requestInfo.TraceIdentifier} {Environment.NewLine}" +
                   $"Headers: {Environment.NewLine}{string.Join($"{Environment.NewLine}", headersAsString)} {Environment.NewLine}" +
                   $"Action Arguments: {Environment.NewLine}{string.Join($"{Environment.NewLine}", actionArgumentsAsString)}";
        }

        public string GenerateResponseLogMessage(ResponseInfo responseInfo)
        {
            return "";
        }
    }
}