using FINAL2.Data;
using FINAL2.Helper;
using FINAL2.Model;
using FINAL2.Services;
using FINAL2.Validators;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Microsoft.AspNetCore.Authorization;

namespace FINAL2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonControler : Controller
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly PersonDbContext _context;
        private readonly AppSettings _appsettings;
        private readonly IConfiguration _config;
        private readonly IUserService _userservice;


        public PersonControler(PersonDbContext context, IOptions<AppSettings> appSettings

            , IUserService userService)
        {
            _context = context;
            _appsettings = appSettings.Value;

            _userservice = userService;

        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Register user)
        {
            var validator = new RegisterValidator();
            var result = validator.Validate(user);
            var errorMessage = "";
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    errorMessage += error.ErrorMessage + " , ";
                }
                return BadRequest(errorMessage);
            }

            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (await _context.Persons.AnyAsync(x => x.Email == user.Email))
                    return Conflict("Email is already registered");
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var adduser = new Person
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = passwordHash,
                    Role = user.Role,
                    Salary = user.Salary,
                    Age = user.Age,
                    IsBlocked = false
                };
                _context.Persons.Add(adduser);
                await _context.SaveChangesAsync();
                Logger.Info($"User {user.UserName} registered successfully");
                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred while registering user");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login loginperson)
        {
            var validator = new LoginValidator();
            var result = validator.Validate(loginperson);
            var errorMessage = "";
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    errorMessage += error.ErrorMessage + " , ";
                }
                return BadRequest(errorMessage);
            }
            var tokenString = await _userservice.LoginPerson(loginperson);
            if (tokenString != null)
            {
                var user = await _context.Persons.FirstOrDefaultAsync(x => x.UserName == loginperson.UserName);

                return Ok(
                    new
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Token = tokenString
                    });
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }
        }



        [Authorize]
        [HttpGet("getall")]
        public async Task<IEnumerable<Person>> Getall()
        {
            return await _userservice.GetAll();
        }
        [Authorize]
        [HttpGet("getuserbyid")]
        public async Task<Person> GetUserbyid(int id)
        {
            return await _userservice.GetUserbyId(id);
        }
    }
}