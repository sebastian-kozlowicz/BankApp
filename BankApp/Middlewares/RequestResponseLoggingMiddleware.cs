﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BankApp.Configuration;
using BankApp.Interfaces.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankApp.Middlewares
{
    public class RequestResponseLoggingMiddleware : IMiddleware
    {
        private readonly IList<string> _ignoredPaths = new List<string>
        {
            "/swagger",
            "/swagger/index.html",
            "/swagger/v1/swagger.json",
            "/swagger/swagger-ui-standalone-preset.js",
            "/swagger/favicon-32x32.png",
            "/swagger/swagger-ui-bundle.js",
            "/swagger/swagger-ui.css",
            "/favicon.ico",
            "/"
        };

        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly LogSanitizationOptions _logSanitizationOptions;
        private readonly ILogSanitizedPayloadBuilder _logSanitizedPayloadBuilder;
        private readonly List<string> _propertyNamesToSanitize = new() { "email" };
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private RequestDelegate _next;
        private Stream _originalResponseBodyStream;

        public RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger,
            IOptions<LogSanitizationOptions> logSanitizationOptions,
            ILogSanitizedPayloadBuilder logSanitizedPayloadBuilder)
        {
            _logger = logger;
            _logSanitizedPayloadBuilder = logSanitizedPayloadBuilder;
            _logSanitizationOptions = logSanitizationOptions.Value;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _next = next;

            try
            {
                if (!IsPathIgnored(context))
                {
                    await LogRequest(context);
                    await LogResponse(context);
                }
                else
                {
                    await _next(context);
                }
            }
            catch (Exception e)
            {
                await LogError(context, e.ToString());
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);

            requestStream.Position = 0;
            var requestBodyText = await new StreamReader(requestStream, Encoding.UTF8).ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (_logSanitizationOptions.IsEnabled)
                requestBodyText =
                    _logSanitizedPayloadBuilder.SanitizePayload(JToken.Parse(requestBodyText),
                        _propertyNamesToSanitize);

            _logger.LogInformation($"Http Request Information: {Environment.NewLine}" +
                                   $"Scheme: {context.Request.Scheme} | " +
                                   $"Host: {context.Request.Host} | " +
                                   $"Path: {context.Request.Path} | " +
                                   $"Query String: {context.Request.QueryString} | " +
                                   $"Request Body: {requestBodyText}");
        }

        private async Task LogResponse(HttpContext context)
        {
            _originalResponseBodyStream = context.Response.Body;
            await using var responseStream = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseStream;

            await _next(context);

            context.Response.Body.Position = 0;
            var responseBodyText = await new StreamReader(context.Response.Body, Encoding.UTF8).ReadToEndAsync();
            context.Response.Body.Position = 0;

            if (_logSanitizationOptions.IsEnabled)
                responseBodyText =
                    _logSanitizedPayloadBuilder.SanitizePayload(JToken.Parse(responseBodyText),
                        _propertyNamesToSanitize);

            _logger.LogInformation($"Http Response Information: {Environment.NewLine}" +
                                   $"Scheme: {context.Request.Scheme} | " +
                                   $"Host: {context.Request.Host} | " +
                                   $"Path: {context.Request.Path} | " +
                                   $"Query String: {context.Request.QueryString} | " +
                                   $"Status Code: {context.Response.StatusCode} | " +
                                   $"Response Body: {responseBodyText}");
            await responseStream.CopyToAsync(_originalResponseBodyStream);
        }

        private async Task LogError(HttpContext context, string errorMessage)
        {
            await using var responseStream = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseStream;

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Unexpected error occurred");

            context.Response.Body.Position = 0;
            await responseStream.CopyToAsync(_originalResponseBodyStream);

            _logger.LogError($"Http Response Information: {Environment.NewLine}" +
                             $"Scheme: {context.Request.Scheme} | " +
                             $"Host: {context.Request.Host} | " +
                             $"Path: {context.Request.Path} | " +
                             $"Query String: {context.Request.QueryString} | " +
                             $"Status Code: {context.Response.StatusCode} | " +
                             $"Error message: {errorMessage}");
        }

        private bool IsPathIgnored(HttpContext context)
        {
            return _ignoredPaths.Contains(context.Request.Path.Value);
        }
    }
}