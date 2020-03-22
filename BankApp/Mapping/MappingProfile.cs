using AutoMapper;
using BankApp.Dtos;
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
        }
    }
}
