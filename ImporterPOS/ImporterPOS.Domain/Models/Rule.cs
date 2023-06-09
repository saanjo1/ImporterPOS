using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class Rule
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public bool IsExecuted { get; set; }

    public bool Active { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<RuleItem> RuleItems { get; } = new List<RuleItem>();

    public virtual ICollection<Taxis> Taxes { get; } = new List<Taxis>();
}
