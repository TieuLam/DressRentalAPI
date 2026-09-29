using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class OrderDetail
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long ItemId { get; set; }

    public string RentalType { get; set; } = null!;

    public long Price { get; set; } = 0;

    public long DepositAmount { get; set; } = 0;

    public virtual ProductItem Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ReturnInspection? ReturnInspection { get; set; }
}
