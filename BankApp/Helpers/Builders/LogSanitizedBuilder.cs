using System.Collections.Generic;
using System.Linq;
using BankApp.Interfaces.Builders;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankApp.Helpers.Builders
{
    public class LogSanitizedBuilder : ILogSanitizedBuilder
    {
        private const string _sanitizedValue = "[Sanitized]";
        private IList<string> _propertyNamesToSanitize;

        public string SanitizePayload(JToken jToken, List<string> propertyNamesToSanitize)
        {
            _propertyNamesToSanitize = propertyNamesToSanitize;

            switch (jToken.Type)
            {
                case JTokenType.Object:
                    SetSanitizedValue((JObject) jToken);
                    break;

                case JTokenType.Array:
                {
                    if (((JArray) jToken).First?.Type == JTokenType.Object)
                        foreach (var arrayItem in (JArray) jToken)
                            if (arrayItem.Type == JTokenType.Object)
                                SetSanitizedValue((JObject) arrayItem);
                    break;
                }
            }

            return jToken.ToString(Formatting.None);
        }

        public List<string> SanitizeHeaders(IHeaderDictionary headers, IList<string> headerNamesToSanitize)
        {
            var sanitizedHeadersAsString = new List<string>();

            foreach (var (key, value) in headers)
                sanitizedHeadersAsString.Add(headerNamesToSanitize.Contains(key)
                    ? $"{key}: {_sanitizedValue}"
                    : $"{key}: {value}");

            return sanitizedHeadersAsString;
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
                        foreach (var jToken in jProperty.Value)
                            if (jToken is JObject childJObject)
                                SetSanitizedValue(childJObject);

                if (_propertyNamesToSanitize.Contains(jProperty.Name))
                    jProperty.Value = _sanitizedValue;
            }
        }
    }
}