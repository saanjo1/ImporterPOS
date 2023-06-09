using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class Print
{
    public Guid Id { get; set; }

    public string? Text { get; set; }

    public string? Type { get; set; }

    public DateTime Created { get; set; }
}
