using CarRentSolution.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class OrderInfo : ComponentBase
{
    [Parameter] public long Id { get; set; }

    private bool isLoaded = false;
    private Order _order;
    private decimal _price;
    private int _days;

    private List<OrderHistory> _histories;

    private Tenant? _tenant;
    private List<RepairCondition> _repairConditions;

    private long userId;

    private int _availableActions; // 0 - ничего, 1 - отклонить/в работе, 2 - отменить/аренда, 3 - открыть аренду
    private string _message;

    protected override async Task OnInitializedAsync()
    {
        if (isLoaded) return;

        _order = await Db
            .Orders
            .Include(c => c.Auto)
            .Include(c => c.Auto.Model)
            .Include(c => c.Auto.Model.Brand)
            .FirstOrDefaultAsync(c => c.Id == Id);
        if (_order == null) Navigation.NavigateTo("/");

        _histories = await Db
            .OrderHistories
            .Include(c => c.Status)
            .Include(c => c.Employee)
            .Where(c => c.OrderId == _order.Id)
            .ToListAsync();

        _availableActions = _histories.Last()
                .StatusId switch
            {
                1 => 1,
                2 => 2,
                4 => 3,
                3  or 5 => 0,
            };

        userId = Convert.ToInt64((await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.FindFirst("Id")
            .Value);

        _days = (_order.DateEndRent.ToDateTime(new TimeOnly(0, 0)) -
                 _order.DateStartRent.ToDateTime(new TimeOnly(0, 0))).Days;
        _price = _order.Auto.RentPrice * _days;

        isLoaded = true;
    }

    private async Task CancelOrder()
    {
        await CreateHistoryRow(5);
        _message = "Заявка отменена";
    }

    private async Task AccessOrder()
    {
        await CreateHistoryRow(2);
        _message = "Заявка одобрена";
    }

    private async Task RejectOrder()
    {
        await CreateHistoryRow(3);
        _message = "Заявка отклонена";
    }
    
    private async Task CreateHistoryRow(long statusId)
    {
        if (Db.OrderHistories.FirstOrDefault(c => c.StatusId == statusId && c.OrderId == Id) != null) return;

        OrderHistory orderHistory = new()
        {
            OrderId = _order.Id,
            EmployeeId = userId,
            StatusId = statusId,
            Date = DateOnly.FromDateTime(DateTime.Today),
            Time = TimeOnly.FromDateTime(DateTime.Now)
        };
        await Db.OrderHistories.AddAsync(orderHistory);
        await Db.SaveChangesAsync();

        Navigation.NavigateTo($"order/{Id}", true);
    }
}