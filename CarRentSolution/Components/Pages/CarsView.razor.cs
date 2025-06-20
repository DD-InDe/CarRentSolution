﻿using CarRentSolution.Entity;
using CarRentSolution.Models;
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

    protected override async Task OnInitializedAsync()
    {
        if (isLoaded) return;

        _brands = await Db.Context.Brands.ToListAsync();
        _autos = await Db
            .Context.Autos
            .Include(c => c.Model)
            .Include(c => c.Model.Brand)
            .Include(c => c.AutoPhotos)
            .ToListAsync();
        _totalCount = _autos.Count;
        isLoaded = true;
    }

    private async Task LoadCars()
    {
        _autos.Clear();
        List<Auto> autos = await Db
            .Context.Autos
            .Include(c => c.Model)
            .Include(c => c.Model.Brand)
            .Include(c => c.AutoPhotos)
            .ToListAsync();

        if (search != String.Empty)
        {
            autos = autos
                .Where(c => c.RentPrice.ToString().Contains(search) ||
                            c.Model.Name.ToLower().Contains(search.ToLower()) ||
                            c.Model.Brand.Name.ToLower().Contains(search.ToLower()))
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
        }
    }

    private void ShowNext()
    {
        if (!IsLastAuto)
        {
            _selectedAuto = _autos[_selectedIndex + 1];
        }
    }

    private void ToEdit(String vin)
    {
        Navigation.NavigateTo($"/update-auto/{vin}");
    }
    
    private void ToCreate()
    {
        Navigation.NavigateTo($"/add-auto");
    }
}