namespace TestBankApi.Data.DTOs;

public class BankTransactionDtoIn
{
    public int Id { get; set; }

    public int AccountId { get; set; } //llave foranea

    public int TransactionType { get; set; }    //llave foranea
        /*
            1. Deposito en efectivo
            3. Deposito via transferencia
            
            2. Retiro en Efectivo
            4. Retiro via transferencia
        */

    public decimal? Amount { get; set; }

    public int? ExternalAccount { get; set; }


}