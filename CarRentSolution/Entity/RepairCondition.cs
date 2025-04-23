using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class RepairCondition
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
}
