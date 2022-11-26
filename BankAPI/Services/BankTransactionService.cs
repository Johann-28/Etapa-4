using BankAPI.Data;
using BankAPI.Data.BankModels;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services;

public class BankTransactionService{

    private readonly BankContext _context;

    public BankTransactionService(BankContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BankTransaction>> GetAll()
    {
        return await _context.BankTransactions.ToListAsync();
    }
}