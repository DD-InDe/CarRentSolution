using CarRentSolution.Entity;
using CarRentSolution.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class OrderView : ComponentBase
{
    private List<OrderStatus> _orderStatusList = new();
    private List<Order> _orders = new();
    private List<Auto> _autos = new();
    private string search { get; set; } = String.Empty;
    private bool isLoaded = false;

    private string autoId;
    private int statusId;

    private int _totalCount = 0;


    protected override async Task OnInitializedAsync()
    {
        if (!AuthService.IsAuthorized) Navigation.NavigateTo("/");
        if (isLoaded) return;

        _orderStatusList = await Db.Context.OrderStatuses.ToListAsync();
        _orders = await Db.Context.Orders.ToListAsync();
        _autos = await Db
            .Context.Autos.Include(c => c.Model)
            .Include(c => c.Model.Brand)
            .ToListAsync();
        _totalCount = _orders.Count;
        isLoaded = true;
    }

    private async Task LoadOrders()
    {
        _orders = await Db
            .Context.Orders.Where(c =>
                c
                    .ClientLastName.ToLower()
                    .Contains(search.ToLower()) ||
                c
                    .ClientFirstName.ToLower()
                    .Contains(search.ToLower()) ||
                (c.ClientMiddleName != null && c
                    .ClientMiddleName.ToLower()
                    .Contains(search.ToLower())) ||
                c.ClientPhone.Contains(search.ToLower()))
            .ToListAsync();

        if (!String.IsNullOrEmpty(autoId) && autoId != "Все")
        {
            _orders = _orders
                .Where(c => c.AutoId == autoId)
                .ToList();
        }
    }
}