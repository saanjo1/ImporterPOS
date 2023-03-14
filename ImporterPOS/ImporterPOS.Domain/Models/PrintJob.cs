using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class PrintJob
{
    public int Id { get; set; }

    public DateTime Created { get; set; }

    public string FileName { get; set; } = null!;
}
