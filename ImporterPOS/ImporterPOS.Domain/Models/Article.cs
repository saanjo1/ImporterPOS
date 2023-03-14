using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class Article
{
    public Guid Id { get; set; }

    public bool Deleted { get; set; }

    public byte[]? Image { get; set; }

    public string? Name { get; set; }

    public string? Tag { get; set; }

    public int ArticleNumber { get; set; }

    public int Order { get; set; }

    public decimal Price { get; set; }

    public Guid? SubCategoryId { get; set; }

    public string? BarCode { get; set; }

    public string? Code { get; set; }

    public decimal ReturnFee { get; set; }

    public int FreeModifiers { get; set; }

    public virtual ICollection<ArticleGood> ArticleGoods { get; } = new List<ArticleGood>();

    public virtual ICollection<ArticleModifier> ArticleModifierArticles { get; } = new List<ArticleModifier>();

    public virtual ICollection<ArticleModifier> ArticleModifierModifiers { get; } = new List<ArticleModifier>();

    public virtual ICollection<InvoiceItemModifier> InvoiceItemModifiers { get; } = new List<InvoiceItemModifier>();

    public virtual ICollection<InvoiceItem> InvoiceItems { get; } = new List<InvoiceItem>();

    public virtual ICollection<RuleItem> RuleItems { get; } = new List<RuleItem>();

    public virtual SubCategory? SubCategory { get; set; }

    public virtual ICollection<Station> Stations { get; } = new List<Station>();

    public virtual ICollection<Taxis> Taxes { get; } = new List<Taxis>();
}
