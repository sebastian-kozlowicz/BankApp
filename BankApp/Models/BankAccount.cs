using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public decimal Balance { get; set; }
    }
}
