using System;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels
{
    public class ChildListModel
    {
        public int ChildListInt { get; set; }
        [SensitiveData] 
        public string ChildListString { get; set; }
        public DateTime ChildListDateTime { get; set; }
        public ComplexListModel ComplexListModel { get; set; }
        [SensitiveData]
        public ComplexListModel ComplexListModelSanitized { get; set; }
    }
}