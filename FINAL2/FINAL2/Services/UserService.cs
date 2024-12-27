using FINAL2.Data;
using FINAL2.Helper;
using FINAL2.Model;
using FINAL2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace FINAL2.Services
{
    public interface IUserService
    {
        Task<string> LoginPerson([FromQuery] Login login);
        Task<IEnumerable<Person>> GetAll();
        Task<Person> GetUserbyId(int id);
    }
}
public class UserService : IUserService
{
    private readonly PersonDbContext _context;
    private readonly AppSettings _appsettings;

    public UserService(PersonDbContext context, IOptions<AppSettings> appsettings)
    {
        _context = context;
        _appsettings = appsettings.Value;
    }

    public async Task<IEnumerable<Person>> GetAll()
    {
        return await _context.Persons.ToListAsync();
    }

    public async Task<Person> GetUserbyId(int id)
    {
        return await _context.Persons.FindAsync(id);
    }

    public async Task<string> LoginPerson([FromBody] Login loginperson)
    {

        var user = await _context.Persons.FirstOrDefaultAsync(x => x.UserName == loginperson.UserName);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginperson.Password, user.Password))
        {
            return null;
        }
        var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,user.Role)
                };
        var tokenString = GenerateToken(authClaims);
        return tokenString;

    }
    private string GenerateToken(List<Claim> claims)
    {
        var key = Encoding.ASCII.GetBytes(_appsettings.Secret);
        var authSecret = new SymmetricSecurityKey(key);
        var tokenObject = new JwtSecurityToken(
            expires: DateTime.Now.AddDays(1),
            claims: claims,
            signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );
        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.WriteToken(tokenObject);
        return token;
    }
}