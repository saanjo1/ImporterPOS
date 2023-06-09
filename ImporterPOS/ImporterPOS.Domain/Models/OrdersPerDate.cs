using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class OrdersPerDate
{
    public DateTime Date { get; set; }

    public int NumberOfOrders { get; set; }
}
