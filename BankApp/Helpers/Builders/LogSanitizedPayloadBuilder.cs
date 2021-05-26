using System.Collections.Generic;
using System.Linq;
using BankApp.Interfaces.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankApp.Helpers.Builders
{
    public class LogSanitizedPayloadBuilder : ILogSanitizedPayloadBuilder
    {
        private IList<string> _propertyNamesToSanitize;
        private const string _sanitizedValue = "[Sanitized]";

        public string SanitizePayload(JToken jToken, List<string> propertyNamesToSanitize)
        {
            _propertyNamesToSanitize = propertyNamesToSanitize;

            switch (jToken.Type)
            {
                case JTokenType.Object:
                    SetSanitizedValue((JObject)jToken);
                    break;

                case JTokenType.Array:
                {
                    if (((JArray)jToken).First.Type == JTokenType.Object)
                    {
                        foreach (var arrayItem in (JArray)jToken)
                            if (arrayItem.Type == JTokenType.Object)
                                SetSanitizedValue((JObject)arrayItem);
                    }
                    break;
                }
            }

            return jToken.ToString(Formatting.None);
        }

        private void SetSanitizedValue(JObject jObject)
        {
            foreach (var jProperty in jObject.Properties())
            {
                var children = jProperty.Children();

                if (children.Any())
                    foreach (var child in children)
                        if (child is JObject childJObject)
                            SetSanitizedValue(childJObject);

                if (jProperty.Value.Type == JTokenType.Array)
                    if (jProperty.First.Type == JTokenType.Object)
                    {
                        foreach (var jToken in jProperty.Value)
                            if (jToken is JObject childJObject)
                                SetSanitizedValue(childJObject);
                    }

                if (_propertyNamesToSanitize.Contains(jProperty.Name))
                    jProperty.Value = _sanitizedValue;
            }
        }
    }
}
