using BankApp.Models.RequestResponseLogging;

namespace BankApp.Interfaces.Builders.Logging
{
    public interface IRequestResponseLoggingBuilder
    {
        string GenerateRequestLogMessage(RequestInfo requestInfo);
        string GenerateResponseLogMessage(ResponseInfo responseInfo);
    }
}