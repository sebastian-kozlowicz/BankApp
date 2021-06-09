using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels.ListModel
{
    public class ComplexChildrenListModel
    {
        public int ComplexChildrenListInt { get; set; }
        [SensitiveData]
        public string ComplexChildrenListString { get; set; }
        public DateTime ComplexChildrenListDateTime { get; set; }
    }
}
