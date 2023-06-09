using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class ArticlesYedek
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
}
