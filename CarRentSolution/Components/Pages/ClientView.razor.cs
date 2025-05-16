using CarRentSolution.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class ClientView : ComponentBase
{
    private List<Tenant> _clients;

    private bool _isLoaded = false;
    private string _search = "";
    private int _totalCount;

    protected override async Task OnInitializedAsync()
    {
        _clients = await Db.Tenants.Include(c => c.TenantDocument).ToListAsync();
        _totalCount = _clients.Count;
        _isLoaded = true;
    }

    private async Task LoadClients()
    {
        _clients = await Db
            .Tenants
            .Include(c => c.TenantDocument)
            .Where(c =>
                c.Phone.Contains(_search) ||
                c
                    .FirstName.ToLower()
                    .Contains(_search.ToLower()) ||
                c
                    .LastName.ToLower()
                    .Contains(_search.ToLower()) ||
                (c.MiddleName != null && c
                    .MiddleName.ToLower()
                    .Contains(_search.ToLower()))
            )
            .ToListAsync();
    }
}