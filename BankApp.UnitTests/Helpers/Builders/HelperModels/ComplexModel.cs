using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels
{
    public class ComplexModel
    {
        public int ComplexInt { get; set; }
        [SensitiveData]
        public string ComplexString { get; set; }
        public DateTime ComplexDateTime { get; set; }
    }
}
