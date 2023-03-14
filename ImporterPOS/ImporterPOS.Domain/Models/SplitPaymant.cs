using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class SplitPaymant
{
    public Guid Id { get; set; }

    public Guid SplitPaymentsId { get; set; }

    public int InvoiceNumber { get; set; }

    public decimal Value { get; set; }

    public Guid? PaymentTypeId { get; set; }

    public virtual PaymentType? PaymentType { get; set; }
}
