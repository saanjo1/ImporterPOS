using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class InvoiceItem
{
    public Guid Id { get; set; }

    public decimal Price { get; set; }

    public bool IsDeleted { get; set; }

    public decimal Quantity { get; set; }

    public decimal BasePrice { get; set; }

    public decimal Total { get; set; }

    public Guid? ArticleId { get; set; }

    public Guid? InvoiceId { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal DiscountAmmount { get; set; }

    public decimal PriceWithoutDiscount { get; set; }

    public decimal BasePriceWithoutDiscount { get; set; }

    public decimal TotalWithoutDiscount { get; set; }

    public decimal TotalTaxes { get; set; }

    public decimal TaxAmmount { get; set; }

    public decimal TotalWithoutTax { get; set; }

    public string? Note { get; set; }

    public int State { get; set; }

    public decimal ReturnFee { get; set; }

    public string? TaxTag { get; set; }

    public decimal? TaxVal { get; set; }

    public string? ArticleName { get; set; }

    public virtual ICollection<InventoryItemBasis> InventoryItemBases { get; } = new List<InventoryItemBasis>();

    public virtual Invoice? Invoice { get; set; }

    public virtual ICollection<InvoiceItemModifier> InvoiceItemModifiers { get; } = new List<InvoiceItemModifier>();
}
