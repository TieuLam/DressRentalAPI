using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class Category
{
    public long Id { get; set; }

    public long TypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual Type Type { get; set; } = null!;
}
