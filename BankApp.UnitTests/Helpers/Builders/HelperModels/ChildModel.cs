using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels
{
    public class ChildModel
    {
        public int ChildInt { get; set; }
        [SensitiveData]
        public string ChildString { get; set; }
        public DateTime ChildDateTime { get; set; }
        public ComplexModel ComplexModel { get; set; }
        [SensitiveData]
        public ComplexModel ComplexModelSanitized { get; set; }
    }
}
