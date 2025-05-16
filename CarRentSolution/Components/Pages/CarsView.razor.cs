using System.Drawing;
using CarRentSolution.Entity;
using CarRentSolution.PageModel;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Components.Pages;

public partial class CarsView : ComponentBase
{
    private List<Auto> _autos = new();
    private List<Brand> _brands;
    private Auto? _selectedAuto { get; set; }

    private string search { get; set; } = String.Empty;
    private int brandId { get; set; }
    private bool isLoaded = false;

    private int _selectedIndex => _autos.IndexOf(_selectedAuto);
    private bool IsFirstAuto => _selectedIndex == 0;
    private bool IsLastAuto => _selectedIndex == _autos.Count - 1;
    private int _totalCount = 0;

    private bool _canCreateOrder = true;

    private PageMessageModel _formMessageModel;

    private Order Order { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (isLoaded) return;

        _brands = await Db.Brands.ToListAsync();
        _autos = await Db
            .Autos
            .Include(c => c.Model)
            .Include(c => c.Model.Brand)
            .ToListAsync();
        _totalCount = _autos.Count;
        isLoaded = true;
    }

    private async Task LoadCars()
    {
        _autos.Clear();
        List<Auto> autos = await Db
            .Autos
            .Include(c => c.Model)
            .Include(c => c.Model.Brand)
            .ToListAsync();

        if (search != String.Empty)
        {
            autos = autos
                .Where(c => c
                                .RentPrice.ToString()
                                .Contains(search) ||
                            c
                                .Model.Name.ToLower()
                                .Contains(search.ToLower()) ||
                            c
                                .Model.Brand.Name.ToLower()
                                .Contains(search.ToLower()))
                .ToList();
        }

        if (brandId != 0)
            autos = autos
                .Where(c => c.Model.BrandId == brandId)
                .ToList();

        _autos.AddRange(autos);
    }

    private void ShowPrevious()
    {
        if (!IsFirstAuto)
        {
            _selectedAuto = _autos[_selectedIndex - 1];
            _canCreateOrder = true;
            _formMessageModel.Clear();
            ClearUpOrder();
        }
    }

    private void ShowNext()
    {
        if (!IsLastAuto)
        {
            _selectedAuto = _autos[_selectedIndex + 1];
            _canCreateOrder = true;
            _formMessageModel.Clear();
            ClearUpOrder();
        }
    }

    private void SelectAuto(Auto auto)
    {
        _selectedAuto = auto;
        ClearUpOrder();
        _formMessageModel = new();
    }

    private void ClearUpOrder()
    {
        Order = new()
        {
            AutoId = _selectedAuto.Vin,
            DateStartRent = DateOnly.FromDateTime(DateTime.Today),
            DateEndRent = DateOnly.FromDateTime(DateTime.Today)
        };
        _canCreateOrder = true;
    }

    private async Task CreateOrder()
    {
        try
        {
            string[] name = Order.ClientFullName.Split(' ');
            if (name.Length < 2 || String.IsNullOrEmpty(Order.ClientFullName))
            {
                _formMessageModel.Change("Фамилия и Имя обязательны для заполнения", MessageType.Error);
                return;
            }

            if (Order.DateStartRent >= Order.DateEndRent)
            {
                _formMessageModel.Change("Начало аренды не может быть раньше или равна окончанию", MessageType.Error);
                return;
            }

            Order.ClientLastName = name[0];
            Order.ClientFirstName = name[1];

            if (name.Length == 3)
                Order.ClientMiddleName = name[2];

            Order.DateCreated = DateOnly.FromDateTime(DateTime.Today);
            await Db.Orders.AddAsync(Order);

            await Db.OrderHistories.AddAsync(new()
            {
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Date = DateOnly.FromDateTime(DateTime.Now),
                StatusId = 1,
                Order = Order
            });

            await Db.SaveChangesAsync();

            _formMessageModel.Change("Заявка создана", MessageType.Ok);

            _canCreateOrder = false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}