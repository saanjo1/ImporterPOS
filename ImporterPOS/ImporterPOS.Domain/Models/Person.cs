using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class Person
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumbers { get; set; }

    public Guid? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }
}
