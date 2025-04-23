using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class AutoPhoto
{
    public long Id { get; set; }

    public string AutoId { get; set; } = null!;

    public string Photo { get; set; } = null!;

    public virtual Auto Auto { get; set; } = null!;
}
