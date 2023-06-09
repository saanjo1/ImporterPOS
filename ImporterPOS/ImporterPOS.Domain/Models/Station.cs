using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class Station
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? OwnerName { get; set; }

    public string? Addres { get; set; }

    public string? TaxNumber { get; set; }

    public string? Currency { get; set; }

    public string? Language { get; set; }

    public string? Country { get; set; }

    public string? Theme { get; set; }

    public bool IsTableService { get; set; }

    public bool PrintPreview { get; set; }

    public bool PrintOrder { get; set; }

    public bool PrintReceiptAfterOrder { get; set; }

    public bool LogOffUserAfterInvoice { get; set; }

    public bool IsFiscal { get; set; }

    public string? InvoiceFooter1 { get; set; }

    public string? InvoiceFooter2 { get; set; }

    public DateTime WorkDayStartAt { get; set; }

    public bool UseProformaInvoice { get; set; }

    public string? PhoneNumber { get; set; }

    public string? CityPostCode { get; set; }

    public string? IdNumber { get; set; }

    public bool ForbidAccessToTakenTables { get; set; }

    public bool IsVat { get; set; }

    public bool IsInvoiceDailyCounter { get; set; }

    public string? LocationMark { get; set; }

    public string? InvoiceMark { get; set; }

    public string? InvoiceFooter3 { get; set; }

    public string? InvoiceFooter4 { get; set; }

    public bool IsRetail { get; set; }

    public bool AllowCancelOrderItems { get; set; }

    public bool PrintOnA4 { get; set; }

    public string? BankAccount { get; set; }

    public string? PlaceOfIssueOfInvoice { get; set; }

    public bool UseAdditionalCurrency { get; set; }

    public string AdditionalCurrencyMark { get; set; } = null!;

    public decimal AdditionalCurrencyRate { get; set; }

    public bool AreInvoiceItemsGrouped { get; set; }

    public bool UseChangeCalculator { get; set; }

    public bool IsTaxAddedOnPrice { get; set; }

    public string? Email { get; set; }

    public bool? DisplayAutomaticallyModifiers { get; set; }

    public bool UsePrintAllOrders { get; set; }

    public bool PrintAfterDeleteArticles { get; set; }

    public bool ShowGrossInsteadOfNettPrice { get; set; }

    public string? Reserved01 { get; set; }

    public int ServiceChargePercentage { get; set; }

    public bool? ShowTaxesOnInvoice { get; set; }

    public bool IsEinvoiceEnabled { get; set; }

    public bool? ShowWaiterNameOnA4invoice { get; set; }

    public bool ForbidEndShiftIfThereAreAnyOrders { get; set; }

    public virtual ICollection<Invoice> Invoices { get; } = new List<Invoice>();

    public virtual ICollection<StationArticle> StationArticles { get; } = new List<StationArticle>();
}
