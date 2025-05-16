namespace CarRentSolution.Entity;

public partial class Rent
{
    public long Id { get; set; }

    public DateOnly DateCreated { get; set; }

    public decimal Penalties { get; set; }

    public decimal FinishPrice { get; set; }

    public DateOnly DateStart { get; set; }

    public DateOnly DateEnd { get; set; }

    public long RepairConditionId { get; set; }

    public string AutoId { get; set; } = null!;

    public long EmployeeId { get; set; }

    public long TenantId { get; set; }

    public long? OrderId { get; set; }

    public virtual Auto Auto { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual RepairCondition RepairCondition { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
}
