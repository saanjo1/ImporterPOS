using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class ArticleModifier
{
    public Guid Id { get; set; }

    public Guid? ModifierId { get; set; }

    public Guid? ArticleId { get; set; }
}
