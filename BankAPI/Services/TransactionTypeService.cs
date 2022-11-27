using BankAPI.Data;
using BankAPI.Data.BankModels;
using Microsoft.EntityFrameworkCore;
using TestBankApi.Data.DTOs;

namespace BankAPI.Services;

public class TransactionTypeService
{
    private readonly BankContext _context;

    public TransactionTypeService(BankContext context)
    {
        _context = context;
    }

    public async Task<TransactionType?> GetById(int id)
    {
        return await _context.TransactionTypes.FindAsync(id);
    }

}