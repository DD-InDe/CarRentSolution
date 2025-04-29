using CarRentSolution.Entity;
using CarRentSolution.Models;
using Microsoft.AspNetCore.Components;

namespace CarRentSolution.Components.Pages;

public partial class OrderInfo : ComponentBase
{
    [Parameter] public long Id { get; set; }

    private Order? _order;
    private bool isLoaded;

    protected override async Task OnInitializedAsync()
    {
        if (!AuthService.IsAuthorized) Navigation.NavigateTo("/");
        if (isLoaded) return;

        _order = await Db.Context.Orders.FindAsync(Id);

        isLoaded = true;
    }
}