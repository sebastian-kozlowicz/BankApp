using AutoMapper;
using BankApp.Dtos.Administrator;
using BankApp.Dtos.ApplicationUser;
using BankApp.Dtos.Auth;
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
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))
                .ForPath(dest => dest.Address.Country, opt => opt.MapFrom(src => src.Address.Country))
                .ForPath(dest => dest.Address.City, opt => opt.MapFrom(src => src.Address.City))
                .ForPath(dest => dest.Address.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForPath(dest => dest.Address.HouseNumber, opt => opt.MapFrom(src => src.Address.HouseNumber))
                .ForPath(dest => dest.Address.ApartmentNumber, opt => opt.MapFrom(src => src.Address.ApartmentNumber))
                .ForPath(dest => dest.Address.PostalCode, opt => opt.MapFrom(src => src.Address.PostalCode))
                .AfterMap((src, dest) => dest.Address.Id = dest.Id);
        }
    }
}
