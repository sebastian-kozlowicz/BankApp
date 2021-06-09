using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels.ObjectModel
{
    public class ChildrenObjectModel
    {
        public int ChildrenObjectInt { get; set; }
        [SensitiveData] 
        public string ChildrenObjectString { get; set; }
        public DateTime ChildrenObjectDateTime { get; set; }
        public ComplexChildrenObjectModel ComplexChildrenObjectModel { get; set; }
        [SensitiveData]
        public ComplexChildrenObjectModel ComplexChildrenObjectModelSanitized { get; set; }
    }
}