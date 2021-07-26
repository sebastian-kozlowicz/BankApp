using System;
using System.Collections.Generic;
using System.Linq;
using BankApp.Interfaces.Helpers.Builders.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BankApp.Helpers.Builders.Logging
{
    public class LogSanitizedBuilder : ILogSanitizedBuilder
    {
        private const string SanitizedValue = "[Sanitized]";
        private IList<string> _propertyNamesToSanitize;

        public string SanitizePayload(JToken jToken, List<string> propertyNamesToSanitize)
        {
            _propertyNamesToSanitize = propertyNamesToSanitize ?? new List<string>();

            switch (jToken.Type)
            {
                case JTokenType.Object:
                    SetSanitizedValueInProperties((JObject) jToken);
                    break;

                case JTokenType.Array:
                {
                    foreach (var arrayItem in jToken)
                        if (arrayItem.Type == JTokenType.Object)
                            SetSanitizedValueInProperties((JObject) arrayItem);
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
                    ? $"{key}: {SanitizedValue}"
                    : $"{key}: {value}");

            return sanitizedHeadersAsString;
        }

        private void SetSanitizedValueInProperties(JObject jObject)
        {
            foreach (var jProperty in jObject.Properties())
            {
                var children = jProperty.Children();

                if (children.Any())
                    foreach (var child in children)
                        if (child is JObject childJObject)
                            SetSanitizedValueInProperties(childJObject);

                if (jProperty.Value.Type == JTokenType.Array)
                    foreach (var arrayItem in jProperty.Value)
                        if (arrayItem is JObject childJObject)
                            SetSanitizedValueInProperties(childJObject);

                if (_propertyNamesToSanitize.Contains(jProperty.Name, StringComparer.OrdinalIgnoreCase))
                    jProperty.Value = SanitizedValue;
            }
        }
    }
}