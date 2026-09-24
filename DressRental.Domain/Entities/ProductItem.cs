using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class ProductItem
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string ItemCode { get; set; } = null!;

    public string Size { get; set; } = null!;

    public string CurrentCondition { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product Product { get; set; } = null!;
}
