using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class ReturnInspection
{
    public long Id { get; set; }

    public long OrderDetailId { get; set; }

    public long? InspectUserId { get; set; }

    public long LateFee { get; set; }

    public long DamageFee { get; set; }

    public string? DamageDescription { get; set; }

    public long RefundedDeposit { get; set; }

    public DateTime InspectAt { get; set; }

    public virtual User? InspectUser { get; set; }

    public virtual OrderDetail OrderDetail { get; set; } = null!;
}
