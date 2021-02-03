using BankApp.Data;
using BankApp.Enumerators;
using System.Linq;

namespace BankApp.Models
{
    public class BankIdentificationNumberData
    {
        public int Id { get; set; }
        public int BankIdentificationNumber { get; set; }
        public IssuingNetwork IssuingNetwork { get; set; }

        private readonly ApplicationDbContext _context;

        public BankIdentificationNumberData()
        {
        }

        public BankIdentificationNumberData(ApplicationDbContext context)
        {
            _context = context;
        }

        public BankIdentificationNumberData GetBankIdentificationNumber(IssuingNetwork issuingNetwork)
        {
            return _context.BankIdentificationNumberData.FirstOrDefault(bin => bin.IssuingNetwork == issuingNetwork);
        }
    }
}
