using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class OrderHistory
{
    public long Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public long OrderId { get; set; }

    public long StatusId { get; set; }

    public long? EmployeeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;
}
