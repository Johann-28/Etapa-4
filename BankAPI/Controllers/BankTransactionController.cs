using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Data.BankModels;
using Microsoft.AspNetCore.Authorization;
using TestBankApi.Data.DTOs;

namespace BankAPI.Controllers;

[Authorize(Policy = "Client")]
[ApiController]
[Route("api/[controller]")]

public class BankTransactionController : ControllerBase
{
    private readonly BankTransactionService bankTransactionService;
    private readonly AccountService accountService;

    private readonly TransactionTypeService transactionTypeService;

 

    public BankTransactionController(BankTransactionService bankTransactionService,
                                     AccountService accountService,
                                     TransactionTypeService transactionTypeService)
    {
        this.bankTransactionService = bankTransactionService;
        this.accountService = accountService;
        this.transactionTypeService = transactionTypeService;
        
    }

    [HttpGet("CheckAccount/{id}")]
    public async Task<IEnumerable<Account>> Get(int id)
    {
        var client = await bankTransactionService.ConsultAccount(id);

        return client;
    }
    [HttpGet("CheckTransactions/{id}")]
    public async Task<IEnumerable<BankTransaction>> GetTrans(int id)
    {
        var trans = await bankTransactionService.ConsultTransactions(id);
        return trans;
    }

    [HttpPost("TakeMoney/cash")]
    public async Task<IActionResult> TakeMoneyCash(BankTransactionDtoIn transactionDtoIn)
    {           
            string validationResult = await ValidateAccount(transactionDtoIn);

            if(!validationResult.Equals("Valid"))
                return BadRequest(new {message = validationResult});
            
            if(transactionDtoIn.TransactionType!=2)
                return BadRequest(new{message = "Esa transaccion no corresponde al retiro en efectivo"});

            if(transactionDtoIn.ExternalAccount is not null)
                 return BadRequest(new {message = "No hace falta una cuenta externa"});

             if(transactionDtoIn.Amount <= 0 )
                return BadRequest(new{ message= " El monto debe ser mayor que cero"});
            

            var account = await accountService.GetById(transactionDtoIn.AccountId);

       
            decimal total = (decimal)(account.Balance - transactionDtoIn.Amount);
            if(total<0)
                return BadRequest(new {message = $"Saldo insuficiente. Saldo: {account.Balance} monto: {transactionDtoIn.Amount}"});
            else 
            {  
                //actualizar cuenta
                AccountDtoIn accountToUpdate = new AccountDtoIn();

                accountToUpdate.AccountType = account.AccountType;
                accountToUpdate.Balance = total;
                accountToUpdate.ClientId = account.ClientId;
                accountToUpdate.Id = account.Id;
                
                await accountService.Update(accountToUpdate);

                //crear registro de transaccion
                var newTransaction = await bankTransactionService.Create(transactionDtoIn);
                return CreatedAtAction(nameof(Get), new{ id = newTransaction.Id}, newTransaction);
            }   
    }

    [HttpPost("TakeMoney/transfer")]
    public async Task<IActionResult>  TakeMoneyTransfer(BankTransactionDtoIn transactionDtoIn)
    {
        string validationResult = await ValidateAccount(transactionDtoIn);

        if(!validationResult.Equals("Valid"))
            return BadRequest(new {message = validationResult});

        if(transactionDtoIn.TransactionType!=4)
            return BadRequest(new { message = "Esa transacción no corresponde al retiro vía transferencia" });

        if(transactionDtoIn.ExternalAccount is null)
            return BadRequest(new {message = "Ingrese una cuenta de donde retirar"});

         if(transactionDtoIn.Amount <= 0 )
                return BadRequest(new{ message= " El monto debe ser mayor que cero"});
            

        var account = await accountService.GetById(transactionDtoIn.AccountId);

        decimal total = (decimal)(account.Balance - transactionDtoIn.Amount);

        if(total < 0)
            return BadRequest(new { message = $"Saldo insuficiente. Saldo: {account.Balance}  monto: {transactionDtoIn.Amount}"});
        else
        {
            //actualizar cuenta
            AccountDtoIn accountToUpdate = new AccountDtoIn();

                accountToUpdate.AccountType = account.AccountType;
                accountToUpdate.Balance = total;
                accountToUpdate.ClientId = account.ClientId;
                accountToUpdate.Id = account.Id;

                await accountService.Update(accountToUpdate);
            
                //crear regiistro de transaccion
                var newTransaction = await bankTransactionService.Create(transactionDtoIn);
                return CreatedAtAction(nameof(Get), new{ id = newTransaction.Id}, newTransaction);
            
        }
    }

    [HttpPost("Deposit")]
    public async Task<IActionResult> Deposit(BankTransactionDtoIn transactionDtoIn)
    {
            string validationResult = await ValidateAccount(transactionDtoIn);

            if(!validationResult.Equals("Valid"))
                return BadRequest(new { message = validationResult});
            
            if(transactionDtoIn.TransactionType != 1)
                return BadRequest(new {message = "Esa transacción no corresponde al déposito en efectivo"});
            
            if(transactionDtoIn.ExternalAccount is not null)
                return BadRequest(new {message = "No hace falta una cuenta externa"});

            var account = await accountService.GetById(transactionDtoIn.AccountId);

            if(transactionDtoIn.Amount > 5000)
                return BadRequest(new{message = "El limite de déposito es de 5000"});

            if(transactionDtoIn.Amount <= 0 )
                return BadRequest(new{ message= " El monto debe ser mayor que cero"});
            
            decimal total = (decimal)(account.Balance + transactionDtoIn.Amount);

            //actualizar cuenta
                AccountDtoIn accountToUpdate = new AccountDtoIn();

                accountToUpdate.AccountType = account.AccountType;
                accountToUpdate.Balance = total;
                accountToUpdate.ClientId = account.ClientId;
                accountToUpdate.Id = account.Id;

                await accountService.Update(accountToUpdate);
            
                //crear regiistro de transaccion
                var newTransaction = await bankTransactionService.Create(transactionDtoIn);
                return CreatedAtAction(nameof(Get), new{ id = newTransaction.Id}, newTransaction);

    }

    [HttpDelete("Delete/{accountId}")]
    public async Task<IActionResult> Delete(int accountId)
    {
        var accountToDelete = await accountService.GetById(accountId);

        if(accountToDelete is not null)
        {
            if(accountToDelete.Balance != 0)
                return BadRequest(new {message = "El saldo necesita ser de 0 para poder eliminar la cuenta"});
           
            await accountService.Delete(accountId);
            return Ok();
        }
        else    
        {
            return NotFound(new {message = $"La cuenta con Id = {accountId} no existe"});
        }
    }



    public async Task<string> ValidateAccount(BankTransactionDtoIn transactionDtoIn)
    {
        string result = "Valid";

        var account = await accountService.GetById(transactionDtoIn.AccountId);

        if(account is null)
            return $"La cuenta {transactionDtoIn.AccountId} no existe";
        
        var accountType = await transactionTypeService.GetById(transactionDtoIn.TransactionType);

        if(accountType is null)
            return $"El tipo de transaccion {transactionDtoIn.TransactionType} no existe";

        return result;
    }
    
}