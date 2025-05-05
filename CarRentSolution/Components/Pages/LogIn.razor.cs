using System.Security.Claims;
using CarRentSolution.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class LogIn : ComponentBase
{
    [SupplyParameterFromForm] private LogInModel _logInModel { get; set; } = new();

    private string _message = String.Empty;

    private async Task Authorize()
    {
        if (await Db
                .Employees
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

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, employee.Email), new(ClaimTypes.Role, role), new("Name", employee.FirstName),
                new("Id", employee.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            HttpContext httpContext = HttpContextAccessor.HttpContext;
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            if (httpContext.Request.Query.Count == 0) Navigation.NavigateTo("/");
        }
        else
            _message = "Пользователь не найден";
    }
}