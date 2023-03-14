using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class Supplier
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? VatNumber { get; set; }

    public bool IsDeleted { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<InventoryDocument> InventoryDocuments { get; } = new List<InventoryDocument>();
}
