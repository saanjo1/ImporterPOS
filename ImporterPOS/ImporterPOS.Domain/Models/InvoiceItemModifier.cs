using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class InvoiceItemModifier
{
    public Guid Id { get; set; }

    public decimal PriceWithoutDiscount { get; set; }

    public Guid? ModifierId { get; set; }

    public Guid? InvoiceItemId { get; set; }

    public virtual InvoiceItem? InvoiceItem { get; set; }
}
