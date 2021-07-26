using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BankApp.Attributes;
using BankApp.Interfaces.Helpers.Builders.Logging;

namespace BankApp.Helpers.Builders.Logging
{
    public class SensitiveDataPropertyNamesBuilder : ISensitiveDataPropertyNamesBuilder
    {
        private List<string> _sensitiveProperties;

        public List<string> GetSensitivePropertiesFromObject(object value)
        {
            _sensitiveProperties = new List<string>();

            var type = value.GetType();
            AddSensitivePropertyInfoNamesToList(type.GetProperties());

            return _sensitiveProperties;
        }

        private void AddSensitivePropertyInfoNamesToList(IEnumerable<PropertyInfo> propertyInfos)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                var type = propertyInfo.PropertyType;

                if (type.BaseType != typeof(ValueType))
                {
                    var nestedProperties = type.GetProperties();
                    if (nestedProperties.Length != 0)
                        AddSensitivePropertyInfoNamesToList(nestedProperties);
                }

                var attributes = propertyInfo.GetCustomAttributes();

                if (attributes.Any(a => a is SensitiveDataAttribute))
                    _sensitiveProperties.Add(propertyInfo.Name);
            }
        }
    }
}