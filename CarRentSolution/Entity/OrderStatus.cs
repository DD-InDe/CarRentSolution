using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class OrderStatus
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();
}
