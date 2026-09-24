using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class ProductImage
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsThumbnail { get; set; }

    public int DisplayOrder { get; set; }

    public virtual Product Product { get; set; } = null!;
}
