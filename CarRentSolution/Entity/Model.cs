using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class Model
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long BrandId { get; set; }

    public virtual ICollection<Auto> Autos { get; set; } = new List<Auto>();

    public virtual Brand Brand { get; set; } = null!;
}
