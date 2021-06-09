using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels.ObjectModel
{
    public class ChildObjectModel
    {
        public int ChildObjectInt { get; set; }
        [SensitiveData]
        public string ChildObjectString { get; set; }
        public DateTime ChildObjectDateTime { get; set; }
        public ComplexObjectModel ComplexObjectModel { get; set; }
        [SensitiveData]
        public ComplexObjectModel ComplexObjectModelSanitized { get; set; }
    }
}
