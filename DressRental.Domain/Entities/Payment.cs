using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class Payment
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentType { get; set; } = null!;

    public long Amount { get; set; }

    public string? TransactionCode { get; set; }

    public DateTime PaidAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
