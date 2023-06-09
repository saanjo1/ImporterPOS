using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models1;

public partial class User
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public bool AllowBackoffice { get; set; }

    public bool AllowEdit { get; set; }

    public string? Password { get; set; }

    public DateTime Created { get; set; }

    public DateTime Modified { get; set; }

    public int UserNumber { get; set; }

    public Guid? GroupId { get; set; }

    public bool AllowViewInvoices { get; set; }

    public bool AllowEndShift { get; set; }

    public bool AllowAccessToTakenTables { get; set; }

    public string? TaxNumber { get; set; }

    public bool IsDeleted { get; set; }

    public bool? AllowDiscount { get; set; }

    public bool AllowSettings { get; set; }

    public bool? AllowCancelOrderItems { get; set; }

    public bool OnlyMyRevenue { get; set; }

    public bool? AllowAccesToStorage { get; set; }

    public bool? AllowAccestToReports { get; set; }

    public bool? AllowVoidInvoice { get; set; }

    public bool? AllowMoveToTable { get; set; }

    public virtual UserGroup? Group { get; set; }

    public virtual ICollection<Invoice> InvoiceOrderByWaiters { get; } = new List<Invoice>();

    public virtual ICollection<Invoice> InvoiceWaiters { get; } = new List<Invoice>();

    public virtual ICollection<Message> MessageFroms { get; } = new List<Message>();

    public virtual ICollection<Message> MessageTos { get; } = new List<Message>();

    public virtual ICollection<Table> Tables { get; } = new List<Table>();
}
