using BankApp.Models.RequestResponseLogging;

namespace BankApp.Interfaces.Helpers.Builders.Logging
{
    public interface IRequestResponseLoggingBuilder
    {
        string GenerateRequestLogMessage(RequestInfo requestInfo);
        string GenerateResponseLogMessage(ResponseInfo responseInfo);
    }
}