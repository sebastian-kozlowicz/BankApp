using System;
using System.Collections.Generic;
using BankApp.Attributes;

namespace BankApp.UnitTests.Helpers.Builders.HelperModels.ObjectModel
{
    public class ParentObjectModel
    {
        [SensitiveData]
        public int ParentObjectInt { get; set; }
        public string ParentObjectString { get; set; }
        public DateTime ParentObjectDateTime { get; set; }
        public ChildObjectModel ChildObjectModel { get; set; }
        public List<ChildrenObjectModel> ChildrenObjectModel { get; set; }
    }
}
