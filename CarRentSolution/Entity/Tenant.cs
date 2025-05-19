using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class Tenant
{
    public long Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string PassportSerial { get; set; } = null!;

    public string PassportNumber { get; set; } = null!;

    public DateOnly PassportDated { get; set; }

    public string PassportIssued { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();

    public virtual TenantDocument? TenantDocument { get; set; }
}
