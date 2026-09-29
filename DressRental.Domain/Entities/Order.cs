using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class Order
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateOnly? ActualReturnDate { get; set; }

    public long TotalAmount { get; set; } = 0;

    public long TotalDeposit { get; set; } = 0;

    public string Status { get; set; } = null!;
    public DateTime CreateAt { get; set; }

    public virtual CustomerProfile Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
