using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels.ListModel
{
    public class ChildrenListModel
    {
        public int ChildrenListInt { get; set; }
        [SensitiveData] 
        public string ChildrenListString { get; set; }
        public DateTime ChildrenListDateTime { get; set; }
        public ComplexChildrenListModel ComplexChildrenListModel { get; set; }
        [SensitiveData]
        public ComplexChildrenListModel ComplexChildrenListModelSanitized { get; set; }
    }
}