using FINAL2.Data;
using FINAL2.Helper;
using FINAL2.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FINAL2.Services
{
    public interface ILoanservice
    {
        Task DelparticipantbyId(int id);
        Task<ActionResult<IEnumerable<Loan>>> ViewLoans(int personid);
    }

    public class Loanservice : ILoanservice
    {
        private readonly PersonDbContext _context;
        private readonly AppSettings _appsettings;

        public Loanservice(PersonDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appsettings = appSettings.Value;
        }


        public async Task DelparticipantbyId(int id)
        {
            var user = await _context.Persons.FirstOrDefaultAsync(x => x.Id == id);
            var loan = await _context.Loans.FirstOrDefaultAsync(x => x.Id == id);

            if (loan == null)
            {
                throw new Exception("");
            }
            if (loan.Status == Status.InProcess)
            {
                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<ActionResult<IEnumerable<Loan>>> ViewLoans(int personid)
        {
            var loans = await _context.Loans.Where(x => x.PersonId == personid).ToListAsync();
            return loans;
        }
    }
}