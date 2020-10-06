using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Branch;
using BankApp.Dtos.Branch.WithAddress;
using BankApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BranchesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{branchId}", Name = "GetBranch")]
        public ActionResult<BranchDto> GetBranch(int branchId)
        {
            var branch = _context.Branches.Include(b => b.BranchAddress).SingleOrDefault(c => c.Id == branchId);

            if (branch == null)
                return NotFound();

            return Ok(_mapper.Map<Branch, BranchDto>(branch));
        }

        [HttpPost]
        [Route("CreateWithAddress")]
        public ActionResult<BranchDto> CreateBranch([FromBody] BranchWithAddressCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Branches.FirstOrDefault(b => b.BranchCode == model.Branch.BranchCode) != null)
            {
                ModelState.AddModelError("BranchCode", "Branch code is already in use.");
                return BadRequest(ModelState);
            }

            var branch = _mapper.Map<Branch>(model);

            _context.Branches.Add(branch);
            _context.SaveChanges();

            var branchDto = _mapper.Map<Branch, BranchDto>(branch);

            return CreatedAtRoute("GetBranch", new { branchId = branchDto.Id }, branchDto);
        }
    }
}
