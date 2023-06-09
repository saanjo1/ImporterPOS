using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class Taxis
{
    public Guid Id { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public string? Name { get; set; }

    public decimal Value { get; set; }

    public bool IsDeleted { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<TaxArticle> TaxArticles { get; } = new List<TaxArticle>();

    public virtual ICollection<Rule> Rules { get; } = new List<Rule>();
}
