using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class CustomerProfile
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<ConsultationBooking> ConsultationBookings { get; set; } = new List<ConsultationBooking>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User? User { get; set; }
}
