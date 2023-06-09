using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class TaxArticle
{
    public Guid TaxId { get; set; }

    public Guid ArticleId { get; set; }

    public virtual Taxis Tax { get; set; } = null!;
}
