using BankApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankTransferController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BankTransferController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult CreateBankTransfer()
        {
            return Ok();
        }
    }
}
