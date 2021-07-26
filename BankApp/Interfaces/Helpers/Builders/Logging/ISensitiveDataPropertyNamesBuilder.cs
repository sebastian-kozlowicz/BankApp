using System.Collections.Generic;

namespace BankApp.Interfaces.Helpers.Builders.Logging
{
    public interface ISensitiveDataPropertyNamesBuilder
    {
        List<string> GetSensitivePropertiesFromObject(object value);
    }
}