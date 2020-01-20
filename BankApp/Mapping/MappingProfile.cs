using AutoMapper;
using BankApp.Dtos;
using BankApp.Models;

namespace BankApp.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<Account, AccountDto>();
        }
    }
}
