using System;
using System.Collections.Generic;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels
{
    public class ParentModel
    {
        [SensitiveData]
        public int ParentInt { get; set; }
        public string ParentString { get; set; }
        public DateTime ParentDateTime { get; set; }
        public ChildModel ChildModel { get; set; }
        public List<ChildListModel> ChildListModel { get; set; }
    }
}
