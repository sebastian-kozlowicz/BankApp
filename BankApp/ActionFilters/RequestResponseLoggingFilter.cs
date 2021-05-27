using BankApp.Interfaces.Builders;
using BankApp.Models.RequestResponseLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BankApp.ActionFilters
{
    public class RequestResponseLoggingFilter : IActionFilter
    {
        private readonly ILogger<RequestResponseLoggingFilter> _logger;
        private readonly IRequestResponseLoggingBuilder _requestResponseLoggingBuilder;

        public RequestResponseLoggingFilter(ILogger<RequestResponseLoggingFilter> logger,
            IRequestResponseLoggingBuilder requestResponseLoggingBuilder)
        {
            _logger = logger;
            _requestResponseLoggingBuilder = requestResponseLoggingBuilder;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var requestInfo = new RequestInfo
            {
                TraceIdentifier = context.HttpContext.TraceIdentifier,
                Method = context.HttpContext.Request.Method,
                Scheme = context.HttpContext.Request.Scheme,
                Path = context.HttpContext.Request.Path,
                Headers = context.HttpContext.Request.Headers,
                ActionArguments = context.ActionArguments
            };

            var requestLogMessage = _requestResponseLoggingBuilder.GenerateRequestLogMessage(requestInfo);
            _logger.LogInformation(requestLogMessage);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result as ObjectResult;

            var responseInfo = new ResponseInfo
            {
                TraceIdentifier = context.HttpContext.TraceIdentifier,
                Path = context.HttpContext.Request.Path,
                Headers = context.HttpContext.Response.Headers,
                Result = result?.Value
            };

            if (responseInfo.Result == null)
            {
                responseInfo.StatusCode = 500;
            }
            else
            {
                if (result?.StatusCode != null)
                    responseInfo.StatusCode = (int) result.StatusCode;
            }

            responseInfo.ExceptionMessage = context.Exception?.ToString();

            var responseLogMessage = _requestResponseLoggingBuilder.GenerateResponseLogMessage(responseInfo);

            if (responseInfo.IsServerErrorStatusCode)
                _logger.LogError(responseLogMessage);
            else
                _logger.LogInformation(responseLogMessage);
        }
    }
}