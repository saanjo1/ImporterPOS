using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class RuleItem
{
    public Guid Id { get; set; }

    public decimal NewPrice { get; set; }

    public Guid? ArticleId { get; set; }

    public Guid? RuleId { get; set; }

    public virtual Article? Article { get; set; }

    public virtual Rule? Rule { get; set; }
}
