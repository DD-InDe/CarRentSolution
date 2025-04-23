using CarRentSolution.Entity;

namespace CarRentSolution.Services;

public class AuthService
{
    public Employee? AuthEmployee { get; set; }
    public bool IsAuthorized => AuthEmployee != null;
}