using System.Security.Claims;
using CarRentSolution.Entity;
using CarRentSolution.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class LogIn : ComponentBase
{
    [SupplyParameterFromForm] private LogInModel _logInModel { get; set; }

    private string _message = String.Empty;

    protected override void OnInitialized()
    {
        _logInModel = new();
    }

    private async Task Authorize()
    {
        if (await Db
                .Context.Employees
                .Include(c => c.Role)
                .FirstOrDefaultAsync(c =>
                    c.Email == _logInModel.Email && c.Password == _logInModel.Password) is { } employee)
        {
            _message = "Вы успешно вошли";
            string role = employee.RoleId switch
            {
                1 => "Admin",
                2 => "Staff",
            };

            var claims = new List<Claim> { new(ClaimTypes.Name, employee.Email), new(ClaimTypes.Role, role) };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            await HttpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }
        else
            _message = "Пользователь не найден";
    }
}