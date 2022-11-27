using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BankAPI.Data.BankModels;

public partial class BankTransaction
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int TransactionType { get; set; }

    public decimal? Amount { get; set; }

    public int? ExternalAccount { get; set; }

    public DateTime RegDate { get; set; }

    [JsonIgnore]
    public virtual Account Account { get; set; } = null!;

    [JsonIgnore]
    public virtual TransactionType TransactionTypeNavigation { get; set; } = null!;
}
