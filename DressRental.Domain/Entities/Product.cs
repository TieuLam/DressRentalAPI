using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class Product
{
    public long Id { get; set; }

    public long CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Color { get; set; }

    public long OriginalPrice { get; set; }

    public long Price3Days { get; set; }

    public long Price7Days { get; set; }

    public int DepositRate { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
}
