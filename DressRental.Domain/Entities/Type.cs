using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class Type
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
