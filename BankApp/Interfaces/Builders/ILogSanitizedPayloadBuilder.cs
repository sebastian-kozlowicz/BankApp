using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace BankApp.Interfaces.Builders
{
    public interface ILogSanitizedPayloadBuilder
    {
        string SanitizePayload(JToken jToken, List<string> propertyNamesToSanitize);
    }
}