namespace Simple.Finance.Models;

using System;

public record WalletBalance
{
    public long WalletId { get; set; }
    public decimal Balance { get; set; }
}
