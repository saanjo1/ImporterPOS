using System;
using System.Collections.Generic;

namespace ImporterPOS.Domain.Models;

public partial class UserGroup
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();
}
