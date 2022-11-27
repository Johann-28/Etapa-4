using BankAPI.Data;
using BankAPI.Data.BankModels;
using Microsoft.EntityFrameworkCore;
using TestBankApi.Data.DTOs;

namespace BankAPI.Services;

public class BankTransactionService{

    private readonly BankContext _context;

    public BankTransactionService(BankContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Account>> ConsultAccount(int clientId)
    {
        List<Account> account = new List<Account>();
        account  = await _context.Accounts.ToListAsync();

        return account.Where(cuenta => cuenta.ClientId == clientId);

        
    }

    public async Task<IEnumerable<BankTransaction>> ConsultTransactions(int accountId)
    {
        List<BankTransaction> transaction = new List<BankTransaction>();
        transaction = await _context.BankTransactions.ToListAsync();

        return transaction.Where(transaccion => transaccion.AccountId == accountId);
    }

    //WithDraw significa retiro
    public async Task<BankTransaction> Create(BankTransactionDtoIn newTransactionDto)
    {
        var newTransaction = new BankTransaction();

        newTransaction.AccountId = newTransactionDto.AccountId;
        newTransaction.TransactionType = newTransactionDto.TransactionType;
        newTransaction.Amount = newTransactionDto.Amount;
        newTransaction.ExternalAccount = newTransactionDto.ExternalAccount;

        _context.BankTransactions.Add(newTransaction);
        await _context.SaveChangesAsync();

        return newTransaction;
    }
}