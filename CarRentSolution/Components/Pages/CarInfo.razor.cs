using CarRentSolution.Entity;
using CarRentSolution.PageModel;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class CarInfo : ComponentBase
{
    [Parameter] public string? Vin { get; set; }
    [Parameter] public bool? IsView { get; set; }

    private Auto _auto { get; set; }
    private List<Model> _models;
    private bool _isLoaded = false;
    private bool _isView => IsView ?? false;
    private PageMessageModel _message = new();

    protected override async Task OnInitializedAsync()
    {
        if (Vin != null)
        {
            _auto = await Db
                .Autos.Include(c => c.Model)
                .FirstOrDefaultAsync(c => c.Vin == Vin);
        }
        else
            _auto = new(){PassportDated = DateOnly.FromDateTime(DateTime.Today)};

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
            if (_auto.PassportDated > DateOnly.FromDateTime(DateTime.Today) || _auto.PassportDated.Year < 1990)
            {
                _message.Change("Некорректная дата", MessageType.Info);
                return;
            }

            if (await Db.Autos.FindAsync(_auto.Vin) == null)
            {
                await Db.Autos.AddAsync(_auto);
            }
            else
            {
                Db.Autos.Update(_auto);
            }

            if (await Db.SaveChangesAsync() == 1)
            {
                Navigation.NavigateTo("/cars");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _message.Change("Произошла ошибка", MessageType.Error);
        }
    }
}