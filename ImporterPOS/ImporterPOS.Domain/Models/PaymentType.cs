using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class PaymentType
{
    public Guid Id { get; set; }

    public int Order { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Invoice> Invoices { get; } = new List<Invoice>();
}
