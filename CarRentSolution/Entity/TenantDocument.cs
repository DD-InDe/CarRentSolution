using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class TenantDocument
{
    public long Id { get; set; }

    public string? Passport { get; set; }

    public string? DriveLicense { get; set; }

    public virtual Tenant IdNavigation { get; set; } = null!;
}
