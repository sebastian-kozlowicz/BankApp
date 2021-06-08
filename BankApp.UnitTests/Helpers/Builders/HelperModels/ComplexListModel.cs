using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels
{
    public class ComplexListModel
    {
        public int ComplexListInt { get; set; }
        [SensitiveData]
        public string ComplexListString { get; set; }
        public DateTime ComplexListDateTime { get; set; }
    }
}
