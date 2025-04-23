using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class Brand
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
