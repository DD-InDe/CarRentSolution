namespace CarRentSolution.Entity;

public partial class Tenant
{
    public string FullName => $"{LastName} {FirstName} {MiddleName}";
}