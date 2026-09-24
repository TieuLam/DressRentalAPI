using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class User
{
    public long Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual CustomerProfile? CustomerProfile { get; set; }

    public virtual ICollection<ReturnInspection> ReturnInspections { get; set; } = new List<ReturnInspection>();
}
