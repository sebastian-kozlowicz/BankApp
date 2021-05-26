using BankApp.Models.RequestResponseLogging;

namespace BankApp.Interfaces.Builders
{
    public interface IRequestResponseLoggingBuilder
    {
        string GenerateRequestLogMessage(RequestInfo requestInfo);
        string GenerateResponseLogMessage(ResponseInfo responseInfo);
    }
}