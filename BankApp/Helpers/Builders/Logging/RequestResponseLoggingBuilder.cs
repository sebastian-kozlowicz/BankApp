using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BankApp.Configuration;
using BankApp.Interfaces.Builders.Logging;
using BankApp.Models.RequestResponseLogging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankApp.Helpers.Builders.Logging
{
    public class RequestResponseLoggingBuilder : IRequestResponseLoggingBuilder
    {
        private readonly List<string> _headerNamesToSanitize = new() {"Authentication"};
        private readonly LogSanitizationOptions _logSanitizationOptions;
        private readonly ILogSanitizedBuilder _logSanitizedBuilder;
        private readonly ISensitiveDataPropertyNamesBuilder _sensitiveDataPropertyNamesBuilder;

        public RequestResponseLoggingBuilder(IOptions<LogSanitizationOptions> logSanitizationOptions,
            ILogSanitizedBuilder logSanitizedBuilder,
            ISensitiveDataPropertyNamesBuilder sensitiveDataPropertyNamesBuilder)
        {
            _logSanitizedBuilder = logSanitizedBuilder;
            _sensitiveDataPropertyNamesBuilder = sensitiveDataPropertyNamesBuilder;
            _logSanitizationOptions = logSanitizationOptions.Value;
        }

        public string GenerateRequestLogMessage(RequestInfo requestInfo)
        {
            var headersAsString = new List<string>();
            var actionArgumentsAsString = new List<string>();
            var propertyNamesToSanitize = new List<string>();

            if (_logSanitizationOptions.IsEnabled)
                foreach (var value in requestInfo.ActionArguments.Values)
                    propertyNamesToSanitize.AddRange(
                        _sensitiveDataPropertyNamesBuilder.GetSensitivePropertiesFromObject(value));

            if (_logSanitizationOptions.IsEnabled)
            {
                var sanitizedHeaders =
                    _logSanitizedBuilder.SanitizeHeaders(requestInfo.Headers, _headerNamesToSanitize);
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
                        propertyNamesToSanitize);
                    actionArgumentsAsString.Add($"{key}: {sanitizedPayload}");
                }
            else
                foreach (var (key, value) in requestInfo.ActionArguments)
                {
                    var payload = JsonConvert.SerializeObject(value, Formatting.None);
                    actionArgumentsAsString.Add($"{key}: {payload}");
                }

            return $"Http Request Information: {Environment.NewLine}" +
                   $"Trace Identifier: {requestInfo.TraceIdentifier} {Environment.NewLine}" +
                   $"Method: {requestInfo.Method} {Environment.NewLine}" +
                   $"Scheme: {requestInfo.Scheme} {Environment.NewLine}" +
                   $"Path: {requestInfo.Path} {Environment.NewLine}" +
                   $"Headers: {Environment.NewLine}{string.Join($"{Environment.NewLine}", headersAsString)} {Environment.NewLine}" +
                   $"Action Arguments: {Environment.NewLine}{string.Join($"{Environment.NewLine}", actionArgumentsAsString)}";
        }

        public string GenerateResponseLogMessage(ResponseInfo responseInfo)
        {
            var headersAsString = new List<string>();
            var propertyNamesToSanitize = new List<string>();

            if (_logSanitizationOptions.IsEnabled)
            {
                var sanitizedHeaders =
                    _logSanitizedBuilder.SanitizeHeaders(responseInfo.Headers, _headerNamesToSanitize);
                headersAsString = sanitizedHeaders;
            }
            else
            {
                foreach (var (key, value) in responseInfo.Headers)
                    headersAsString.Add($"{key}: {value}");
            }

            string result;

            if (responseInfo.IsServerErrorStatusCode)
            {
                result = responseInfo.ExceptionMessage;
            }
            else if (_logSanitizationOptions.IsEnabled)
            {
                propertyNamesToSanitize.AddRange(
                    _sensitiveDataPropertyNamesBuilder.GetSensitivePropertiesFromObject(responseInfo.Result));

                result = _logSanitizedBuilder.SanitizePayload(JToken.FromObject(responseInfo.Result),
                    propertyNamesToSanitize);
            }
            else
            {
                result = JsonConvert.SerializeObject(responseInfo.Result);
            }

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