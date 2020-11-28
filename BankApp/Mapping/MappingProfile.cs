using AutoMapper;
using BankApp.Dtos.Address;
using BankApp.Dtos.Administrator;
using BankApp.Dtos.ApplicationUser;
using BankApp.Dtos.Auth;
using BankApp.Dtos.BankAccount;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Dtos.Card;
using BankApp.Dtos.Customer;
using BankApp.Dtos.Manager;
using BankApp.Dtos.Teller;
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
            CreateMap<Teller, TellerDto>();
            CreateMap<Manager, ManagerDto>();
            CreateMap<Branch, BranchDto>();
            CreateMap<BranchAddress, AddressDto>();
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))
                .ForPath(dest => dest.UserAddress.Country, opt => opt.MapFrom(src => src.Address.Country))
                .ForPath(dest => dest.UserAddress.City, opt => opt.MapFrom(src => src.Address.City))
                .ForPath(dest => dest.UserAddress.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForPath(dest => dest.UserAddress.HouseNumber, opt => opt.MapFrom(src => src.Address.HouseNumber))
                .ForPath(dest => dest.UserAddress.ApartmentNumber, opt => opt.MapFrom(src => src.Address.ApartmentNumber))
                .ForPath(dest => dest.UserAddress.PostalCode, opt => opt.MapFrom(src => src.Address.PostalCode))
                .AfterMap((src, dest) => dest.UserAddress.Id = dest.Id);

            CreateMap<RegisterByAnotherUserDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.User.Surname))
                .ForPath(dest => dest.UserAddress.Country, opt => opt.MapFrom(src => src.Address.Country))
                .ForPath(dest => dest.UserAddress.City, opt => opt.MapFrom(src => src.Address.City))
                .ForPath(dest => dest.UserAddress.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForPath(dest => dest.UserAddress.HouseNumber, opt => opt.MapFrom(src => src.Address.HouseNumber))
                .ForPath(dest => dest.UserAddress.ApartmentNumber, opt => opt.MapFrom(src => src.Address.ApartmentNumber))
                .ForPath(dest => dest.UserAddress.PostalCode, opt => opt.MapFrom(src => src.Address.PostalCode))
                .AfterMap((src, dest) => dest.UserAddress.Id = dest.Id);

            CreateMap<BranchWithAddressCreationDto, Branch>()
              .ForMember(dest => dest.BranchCode, opt => opt.MapFrom(src => src.Branch.BranchCode))
              .ForPath(dest => dest.BranchAddress.Country, opt => opt.MapFrom(src => src.Address.Country))
              .ForPath(dest => dest.BranchAddress.City, opt => opt.MapFrom(src => src.Address.City))
              .ForPath(dest => dest.BranchAddress.Street, opt => opt.MapFrom(src => src.Address.Street))
              .ForPath(dest => dest.BranchAddress.HouseNumber, opt => opt.MapFrom(src => src.Address.HouseNumber))
              .ForPath(dest => dest.BranchAddress.ApartmentNumber, opt => opt.MapFrom(src => src.Address.ApartmentNumber))
              .ForPath(dest => dest.BranchAddress.PostalCode, opt => opt.MapFrom(src => src.Address.PostalCode))
              .AfterMap((src, dest) => dest.BranchAddress.Id = dest.Id);
        }
    }
}
