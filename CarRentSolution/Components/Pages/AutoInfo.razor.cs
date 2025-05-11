using CarRentSolution.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class AutoInfo : ComponentBase
{
    [Parameter] public string Vin { get; set; }
    [Parameter] public bool IsView { get; set; }

    private Auto _auto { get; set; }
    private List<Model> _models;
    private bool _isLoaded = false;
    private string message { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (_isLoaded) return;

        _auto = await Db
            .Autos.Include(c => c.Model)
            .FirstOrDefaultAsync(c => c.Vin == Vin);
        if (_auto == null) Navigation.NavigateTo("/");

        _models = await Db
            .Models
            .Include(c => c.Brand)
            .ToListAsync();

        _isLoaded = true;
    }

    private async Task Save()
    {
        try
        {
            Db.Autos.Update(_auto);
            if (await Db.SaveChangesAsync() == 1)
            {
                Navigation.NavigateTo("/cars");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            message = "Произошла ошибка";
        }
    }
    
    private void OpenView()=> Navigation.NavigateTo($"update-auto/{Vin}/true");
    private void OpenEdit()=> Navigation.NavigateTo($"update-auto/{Vin}/false");
}