using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class Employee
{
    public long Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public long? RoleId { get; set; }

    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();

    public virtual Role? Role { get; set; }
}
