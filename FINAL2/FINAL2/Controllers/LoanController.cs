using FINAL2.Data;
using FINAL2.Helper;
using FINAL2.Model;
using FINAL2.Services;
using FINAL2.Validators;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NLog;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Data;

namespace FINAL2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly PersonDbContext _context;
        private readonly AppSettings _appsettings;

        private readonly ILoanservice _loanservice;
        private readonly IConfiguration _config;

        public LoanController(PersonDbContext context, IOptions<AppSettings> appsettings,
            IConfiguration config, ILoanservice loanservice)
        {
            _context = context;
            _appsettings = appsettings.Value;

            _config = config;
            _loanservice = loanservice;
        }
        [Authorize(Roles = Role.Accountant)]
        [HttpGet("viewloans")]
        public async Task<ActionResult<IEnumerable<Loan>>> Viewloans(int personid)
        {

            return await _context.Loans.ToListAsync();
        }
        [Authorize(Roles = Role.Accountant)]
        [HttpPost("block/{type}/{id}/{period}")]
        public async Task<ActionResult> BlockUserOrLoan(string type, int id, int period)
        {
            try
            {
                if (type.ToLower() == "user")
                {
                    var user = await _context.Persons.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    user.IsBlocked = true;
                }
                else if (type.ToLower() == "loan")
                {
                    var loan = await _context.Loans.FindAsync(id);
                    if (loan == null)
                    {
                        return NotFound();
                    }

                }
                else
                {
                    return BadRequest("Invalid block type. Use 'user' or 'loan'.");
                }

                await _context.SaveChangesAsync();
                return Ok($"Successfully blocked {type} for {period} days.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.Id == id);
        }






        [Authorize]
        [HttpPost("addloan")]
        public async Task<ActionResult<Loan>> AddLoan(int id, Loan loan)
        {

            try
            {
                var validator = new LoanValidator();

                var result = validator.Validate(loan);
                var errorMessage = "";
                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        errorMessage += error.ErrorMessage + " , ";
                    }
                    return BadRequest(errorMessage);
                }

                if (!result.IsValid)
                {
                    return BadRequest(result.Errors.Select(e => e.ErrorMessage));
                }
                var person = _context.Persons.FirstOrDefault(x => x.Id == id);
                if (person.IsBlocked)
                {
                    return Unauthorized();
                }

                var existingLoan = await _context.Loans.FirstOrDefaultAsync(x => x.PersonId == id);


                var newLoan = new Loan
                {
                    Type = (LoanType)loan.Type,
                    Amount = loan.Amount,
                    Currency = (Currency)loan.Currency,
                    Period = loan.Period,
                    Status = (Status)loan.Status,
                    PersonId = loan.PersonId
                };

                _context.Loans.Add(newLoan);
                await _context.SaveChangesAsync();

                return Ok(newLoan);
            }
            catch (DbUpdateException ex)
            {
                Logger.Error(ex, "Cannot add loan");
                return BadRequest($"Cannot add loan: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Logger.Error(ex, "Invalid enum value");
                return BadRequest($"Invalid enum value: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<IEnumerable<Person>>> DelparticipantById(int id)
        {
            try
            {
                await _loanservice.DelparticipantbyId(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize]
        [HttpPut("updateuser/{id}")]
        public async Task<ActionResult<Person>> UpdateParticipantById(int id, int loanid, Loan updatedLoan)
        {
            try
            {
                var user = await _context.Persons.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return Unauthorized();
                }

                var loan = await _context.Loans.FirstOrDefaultAsync(x => x.Id == id);

                if (loan == null)
                {
                    return NotFound();
                }
                if (User.IsInRole("User") && user.Id != loan.PersonId)
                {

                    return Forbid();
                }

                if (user.Role == "User" && loan.Status == Status.InProcess)
                {

                    loan.Amount = updatedLoan.Amount;
                    loan.Currency = updatedLoan.Currency;
                    loan.Period = updatedLoan.Period;
                    loan.Id = updatedLoan.Id;
                    return Ok("Loan was udated by user succes");
                }
                else if (user.Role == "Accountant")
                {
                    loan.Amount = updatedLoan.Amount;
                    loan.Currency = updatedLoan.Currency;
                    loan.Period = updatedLoan.Period;
                    loan.Id = updatedLoan.Id;
                    updatedLoan.Type = loan.Type;
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                    return Ok("Loan was updated by accountant successfuly");
                }
                return BadRequest("loan cannot be updated");



            }



            catch (Exception exception)
            {
                return BadRequest($" Cannot update loan an error occured :{exception.Message}");

            }
        }

    }
}