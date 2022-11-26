using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services;

public class LoginService
{
    private readonly BankContext _context;

    public LoginService(BankContext context)
    {
        _context = context;
    }

    public async Task<Administrator?> GetAdmin(AdminDTO admin)
    {
        return await _context.Administrators.SingleOrDefaultAsync(x => x.Email == admin.Email && x.Pwd == admin.Pwd);
    }

    public async Task<Client?> GetClient(ClientDTO client){
        return await _context.Clients.SingleOrDefaultAsync( c => c.Email == client.Email && c.Pwd == client.Pwd);
    }


}