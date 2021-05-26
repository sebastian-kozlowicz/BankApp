using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace BankApp.Interfaces.Builders
{
    public interface ILogSanitizedBuilder
    {
        string SanitizePayload(JToken jToken, List<string> propertyNamesToSanitize);
        List<string> SanitizeHeaders(IHeaderDictionary headers, IList<string> headerNamesToSanitize);
    }
}