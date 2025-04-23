using CarRentSolution.Entity;
using CarRentSolution.Models;
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
        if(!AuthService.IsAuthorized) Navigation.NavigateTo("/");
        if(_isLoaded) return;

        _models = await Db.Context.Models
            .Include(c=>c.Brand)
            .ToListAsync();
        _auto = new();
        _isLoaded = true;
    }

    private async Task Save()
    {
        try
        {
            await Db.Context.Autos.AddAsync(_auto);
            if (await Db.Context.SaveChangesAsync() == 1)
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