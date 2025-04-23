using CarRentSolution.Entity;
using CarRentSolution.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class LogIn : ComponentBase
{
    [CascadingParameter] public HttpContext? HttpContext { get; set; } = default!;
    [SupplyParameterFromForm] private LogInModel _logInModel { get; set; }

    private string _message = String.Empty;

    protected override void OnInitialized()
    {
        _logInModel = new();
    }

    private async Task Authorize()
    {
        Employee? employee = await Db
            .Context.Employees
            .Include(c => c.Role)
            .FirstOrDefaultAsync(c =>
                c.Email == _logInModel.Email && c.Password == _logInModel.Password);
        if (employee == null)
        {
            _message = "Пользователь не найден";
            return;
        }

        AuthService.AuthEmployee = employee;
        Navigation.NavigateTo("/");
    }
}