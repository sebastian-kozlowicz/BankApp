using AutoMapper;
using BankApp.Dtos.Administrator;
using BankApp.Dtos.ApplicationUser;
using BankApp.Dtos.BankAccount;
using BankApp.Dtos.Card;
using BankApp.Dtos.Customer;
using BankApp.Dtos.Employee;
using BankApp.Dtos.Manager;
using BankApp.Models;

namespace BankApp.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BankAccount, BankAccountDto>();
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<Card, CardDto>();
            CreateMap<Administrator, AdministratorDto>();
            CreateMap<Customer, CustomerDto>();
            CreateMap<Employee, EmployeeDto>();
            CreateMap<Manager, ManagerDto>();
        }
    }
}
