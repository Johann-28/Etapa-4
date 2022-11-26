using Microsoft.AspNetCore.Mvc;
using BankAPI.Data.DTOs;
using BankApi.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BankAPI.Data.BankModels;

namespace BankAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class LoginController : ControllerBase
{
    private readonly LoginService loginService;
    private IConfiguration config;
    public LoginController(LoginService loginService, IConfiguration config)
    {
        this.loginService = loginService;
        this.config = config;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Login(AdminDTO adminDTO)
    {
        var admin = await loginService.GetAdmin(adminDTO);

        if(admin is null)
            return BadRequest(new {mesagge = "Credenciales inválidas."});
        

        string jwtToken = GenerateToken(admin);
        return Ok(new{token = jwtToken});
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginClient(ClientDTO clientDTO)
    {
        var client = await loginService.GetClient(clientDTO);

        if(client is null)
            return BadRequest(new {message = "Credenciales inválidas."});

        string jwtToken = GenerateTokenClient(client);

        return Ok(new{token = jwtToken});
    }

    private string GenerateToken(Administrator admin)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim("AdminType", admin.AdminType)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var securityToken = new JwtSecurityToken(
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(60),
                            signingCredentials: creds);

       string token = new JwtSecurityTokenHandler().WriteToken(securityToken);  

        return token;
    }   

    private string GenerateTokenClient(Client client)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, client.Name),
            new Claim(ClaimTypes.Email,client.Email),
            new Claim("Email", client.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key2").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var securityToken = new JwtSecurityToken(
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(60),
                            signingCredentials: creds);

        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }

}