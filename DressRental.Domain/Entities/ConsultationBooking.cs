using System;
using System.Collections.Generic;

namespace DressRental.Domain.Entities;

public partial class ConsultationBooking
{
    public long Id { get; set; }

    public long CustomerId { get; set; }

    public DateTime AppointmentTime { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public virtual CustomerProfile Customer { get; set; } = null!;
}
