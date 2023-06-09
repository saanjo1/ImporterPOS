using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class Invoice
{
    public Guid Id { get; set; }

    public DateTime DateCreated { get; set; }

    public int InvoiceNumber { get; set; }

    public bool IsOrder { get; set; }

    public bool IsProformaInvoice { get; set; }

    public decimal Total { get; set; }

    public string? Name { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsClosed { get; set; }

    public DateTime WorkingDay { get; set; }

    public string? Error { get; set; }

    public Guid? WaiterId { get; set; }

    public Guid? PaymentTypeId { get; set; }

    public Guid? TableId { get; set; }

    public Guid? StationId { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal DiscountAmmount { get; set; }

    public Guid? CustomerId { get; set; }

    public string? Note { get; set; }

    public bool IsStorned { get; set; }

    public string? Jir { get; set; }

    public string? Zki { get; set; }

    public decimal ReturnChange { get; set; }

    public string? CashPayed { get; set; }

    public string? InvoiceNumberFormatted { get; set; }

    public DateTime OrderCreated { get; set; }

    public Guid? OrderByWaiterId { get; set; }

    public int ServiceChargePercentage { get; set; }

    public string? FiscalReceiptNumber { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Einvoice? Einvoice { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; } = new List<InvoiceItem>();

    public virtual User? OrderByWaiter { get; set; }

    public virtual PaymentType? PaymentType { get; set; }

    public virtual Station? Station { get; set; }

    public virtual Table? Table { get; set; }

    public virtual User? Waiter { get; set; }
}
