using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class Sector
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public bool Deleted { get; set; }

    public int Order { get; set; }

    public virtual ICollection<Table> Tables { get; } = new List<Table>();
}
