using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class AutoStatus
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Auto> Autos { get; set; } = new List<Auto>();
}
