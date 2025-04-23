using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class Order
{
    public long Id { get; set; }

    public DateOnly DateCreated { get; set; }

    public DateOnly DateStartRent { get; set; }

    public DateOnly DateEndRent { get; set; }

    public string ClientLastName { get; set; } = null!;

    public string ClientFirstName { get; set; } = null!;

    public string? ClientMiddleName { get; set; }

    public string ClientPhone { get; set; } = null!;

    public string AutoId { get; set; } = null!;

    public virtual Auto Auto { get; set; } = null!;

    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();
}
