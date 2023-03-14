using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class InventoryItemBasis
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public decimal Quantity { get; set; }

    public bool IsDeleted { get; set; }

    public decimal? Price { get; set; }

    public decimal? Tax { get; set; }

    public decimal? Total { get; set; }

    public string Discriminator { get; set; } = null!;

    public Guid? InventoryDocumentId { get; set; }

    public Guid? StorageId { get; set; }

    public Guid? GoodId { get; set; }

    public Guid? InvoiceItemId { get; set; }

    public decimal? NormativQuantity { get; set; }

    public decimal? CurrentQuantity { get; set; }

    public decimal? Refuse { get; set; }

    public virtual Good? Good { get; set; }

    public virtual InventoryDocument? InventoryDocument { get; set; }

    public virtual InvoiceItem? InvoiceItem { get; set; }

    public virtual Storage? Storage { get; set; }
}
