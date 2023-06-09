using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class MeasureUnit
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Good> Goods { get; } = new List<Good>();
}
