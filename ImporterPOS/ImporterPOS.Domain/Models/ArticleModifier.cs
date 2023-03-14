using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class ArticleModifier
{
    public Guid Id { get; set; }

    public Guid? ModifierId { get; set; }

    public Guid? ArticleId { get; set; }

    public virtual Article? Article { get; set; }

    public virtual Article? Modifier { get; set; }
}
