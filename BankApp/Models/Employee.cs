using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp.Models
{
    public class Employee
    {
        public string Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
