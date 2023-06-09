using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class Einvoice
{
    public Guid InvoiceId { get; set; }

    public int StatusCode { get; set; }

    public DateTime StatusTimestamp { get; set; }

    public string? StatusNote { get; set; }

    public string? Note { get; set; }

    public string SenderBankAccountNumber { get; set; } = null!;

    public Guid? SenderId { get; set; }

    public Guid? ReceiverId { get; set; }

    public Guid? OrderFormFileId { get; set; }

    public bool IsStationVat { get; set; }

    public string WaiterName { get; set; } = null!;

    public string? OrderFormReference { get; set; }

    public string? OrderFormFileName { get; set; }

    public int? PaymentDue { get; set; }

    public string? PaymentIdModel { get; set; }

    public string? PaymentIdReferenceNumber { get; set; }

    public string? PaymentTerms { get; set; }

    public string RegisterId { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual EinvoiceOrderFormFile? OrderFormFile { get; set; }

    public virtual EinvoiceParty? Receiver { get; set; }

    public virtual EinvoiceParty? Sender { get; set; }
}
