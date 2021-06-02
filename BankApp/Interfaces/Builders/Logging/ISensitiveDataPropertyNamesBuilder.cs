using System.Collections.Generic;

namespace BankApp.Interfaces.Builders.Logging
{
    public interface ISensitiveDataPropertyNamesBuilder
    {
        List<string> GetSensitivePropertiesFromObject(object value);
    }
}