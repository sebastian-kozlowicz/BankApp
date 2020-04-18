﻿using Microsoft.AspNetCore.Identity;

namespace BankApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Administrator Administrator { get; set; }
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
        public Manager Manager { get; set; }
        public Address Address { get; set; }
    }
}
