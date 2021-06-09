using System;
using System.Collections.Generic;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels.ListModel
{
    public class ParentListModel
    {
        [SensitiveData]
        public int ParentListInt { get; set; }
        public string ParentListString { get; set; }
        public DateTime ParentListDateTime { get; set; }
        public ChildListModel ChildListModel { get; set; }
        public List<ChildrenListModel> ChildrenListModel { get; set; }
    }
}
