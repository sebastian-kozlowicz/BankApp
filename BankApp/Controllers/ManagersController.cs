using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Manager;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ManagersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ManagerDto>> GetManagers()
        {
            var managers = _context.Managers.Include(m => m.ApplicationUser).ToList();

            if (managers == null)
                return NotFound();

            return _mapper.Map<List<Manager>, List<ManagerDto>>(managers);
        }

        [HttpGet("{userId}", Name = "GetManager")]
        public ActionResult<ManagerDto> GetManager(string userId)
        {
            var manager = _context.Managers.Include(m => m.ApplicationUser).SingleOrDefault(m => m.Id == userId);

            if (manager == null)
                return NotFound();

            return _mapper.Map<Manager, ManagerDto>(manager);
        }
    }
}