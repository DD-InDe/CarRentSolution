using CarRentSolution.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class CreateAuto : ComponentBase
{
    private Auto _auto { get; set; }
    private List<Model> _models;
    private bool _isLoaded = false;
    private string message { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if(_isLoaded) return;

        _models = await Db.Models
            
            .Include(c=>c.Brand)
            .ToListAsync();
        _auto = new();
        _isLoaded = true;
    }

    private async Task Save()
    {
        try
        {
            await Db.Autos.AddAsync(_auto);
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
}