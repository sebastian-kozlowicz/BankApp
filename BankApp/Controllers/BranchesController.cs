using AutoMapper;
using BankApp.Data;
using BankApp.Dtos.Branch.WithAddress;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("CreateWithAddress")]
        public ActionResult CreateBranch([FromBody] BranchWithAddressCreationDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();
        }
    }
}
