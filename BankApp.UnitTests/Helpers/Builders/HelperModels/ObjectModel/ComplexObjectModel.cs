using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels.ObjectModel
{
    public class ComplexObjectModel
    {
        public int ComplexObjectInt { get; set; }
        [SensitiveData]
        public string ComplexObjectString { get; set; }
        public DateTime ComplexObjectDateTime { get; set; }
    }
}
