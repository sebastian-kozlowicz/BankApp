using AutoMapper;
using BankApp.Data;
using BankApp.Dtos;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AdministratorsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<AdministratorDto> GetAdministrators()
        {
            var administrators = _context.Administrators.Include(a => a.ApplicationUser).ToList();
            return _mapper.Map<List<Administrator>, List<AdministratorDto>>(administrators);
        }

        [HttpGet("{userId}")]
        public AdministratorDto GetAdministrator(string userId)
        {
            var administrator = _context.Administrators.Include(a => a.ApplicationUser).SingleOrDefault(a => a.Id == userId);
            return _mapper.Map<Administrator, AdministratorDto>(administrator);
        }
    }
}