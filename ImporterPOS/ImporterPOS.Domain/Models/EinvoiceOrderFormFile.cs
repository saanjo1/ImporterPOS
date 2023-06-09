using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class EinvoiceOrderFormFile
{
    public Guid Id { get; set; }

    public byte[]? GzippedContents { get; set; }

    public virtual ICollection<Einvoice> Einvoices { get; } = new List<Einvoice>();
}
