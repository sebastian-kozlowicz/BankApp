using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels.ObjectModel
{
    public class ComplexChildrenObjectModel
    {
        public int ComplexChildrenObjectInt { get; set; }
        [SensitiveData]
        public string ComplexChildrenObjectString { get; set; }
        public DateTime ComplexChildrenObjectDateTime { get; set; }
    }
}
