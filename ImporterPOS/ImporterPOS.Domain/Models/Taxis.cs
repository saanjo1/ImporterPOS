using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class Taxis
{
    public Guid Id { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public string? Name { get; set; }

    public decimal Value { get; set; }

    public bool IsDeleted { get; set; }

    public string? Description { get; set; }

    public int CategoryType { get; set; }

    public string? Label { get; set; }

    public int GroupId { get; set; }

    public virtual ICollection<Article> Articles { get; } = new List<Article>();

    public virtual ICollection<Rule> Rules { get; } = new List<Rule>();
}
