using Aspose.Words;
using CarRentSolution.Entity;
using CarRentSolution.PageModel;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CarRentSolution.Components.Pages;

public partial class RentInfo : ComponentBase
{
    [Parameter] public long? OrderId { get; set; }
    private bool _isView = false;
    private bool _isLoaded = false;
    private Rent _rent;
    private List<Tenant> _tenants;
    private List<Auto> _cars;
    private List<RepairCondition> _repairConditions;
    private PageMessageModel _message = new();
    private long _employeeId;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (OrderId != null)
            {
                Order order = await Db.Orders.FindAsync(OrderId);
                if (order == null)
                {
                    Navigation.NavigateTo("/");
                    return;
                }

                _rent = await Db.Rents
                    .Include(c => c.Tenant)
                    .Include(c => c.RepairCondition)
                    .Include(c => c.Order)
                    .FirstOrDefaultAsync(c => c.OrderId == OrderId) ?? new Rent()
                {
                    OrderId = OrderId,
                    AutoId = order.AutoId,
                    DateStart = order.DateStartRent,
                    DateEnd = order.DateEndRent
                };
            }
            else
            {
                _rent = new()
                {
                    DateStart = DateOnly.FromDateTime(DateTime.Now),
                    DateEnd = DateOnly.FromDateTime(DateTime.Now)
                };
            }

            if (_rent.Id != 0) _isView = true;

            _cars = await Db.Autos
                .Include(c => c.Model)
                .Include(c => c.Model.Brand)
                .ToListAsync();
            await UpdateListClient();
            _repairConditions = await Db.RepairConditions.ToListAsync();

            _employeeId = Convert.ToInt64((await AuthenticationStateProvider.GetAuthenticationStateAsync()).User
                .FindFirst("Id")
                .Value);

            _isLoaded = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }


    private async Task UpdateListClient()
    {
        _tenants = await Db.Tenants.Include(c => c.TenantDocument).ToListAsync();
    }

    private bool CheckNullDocuments(long id)
    {
        if (_rent.Id != 0)
        {
            return false;
        }
        
        Tenant tenant = _tenants.First(c => c.Id == id);
        return tenant.TenantDocument?.Passport == null || tenant.TenantDocument?.DriveLicense == null;
    }

    private async Task<bool> CheckCarStatus(string vin)
    {
        if (_rent.Id != 0)
        {
            return false;
        }

        Auto auto = _cars.First(c => c.Vin == vin);

        bool rentBusy = await Db.Rents.AnyAsync(c =>
            c.AutoId == vin && (_rent.DateEnd > c.DateStart || c.DateEnd > _rent.DateStart));

        bool check = rentBusy || auto.StatusId == 3 || auto.StatusId == 4;
        return check;
    }

    private async Task SaveRent()
    {
        try
        {
            if (_rent.Id == 0)
            {
                if (_rent.DateStart > _rent.DateEnd)
                {
                    _message.Change("Дата начала не может быть больше или равна окончанию", MessageType.Error);
                    return;
                }

                if (CheckNullDocuments(_rent.TenantId))
                {
                    _message.Change("Нельзя оформить аренду без копий документов у арендатора", MessageType.Error);
                    return;
                }

                if (await CheckCarStatus(_rent.AutoId))
                {
                    _message.Change("Данный автомобиль недоступен в этот период", MessageType.Error);
                    return;
                }

                _rent.EmployeeId = _employeeId;
                _rent.DateCreated = DateOnly.FromDateTime(DateTime.Now);
                _rent.FinishPrice = GetPrice().Item1;

                await Db.Rents.AddAsync(_rent);

                if (OrderId != null)
                {
                    await Db.OrderHistories.AddAsync(new OrderHistory()
                    {
                        OrderId = OrderId.Value,
                        EmployeeId = _employeeId,
                        StatusId = 4,
                        Date = DateOnly.FromDateTime(DateTime.Today),
                        Time = TimeOnly.FromDateTime(DateTime.Now)
                    });
                }

                if (await Db.SaveChangesAsync() > 0)
                {
                    _isView = true;
                    _message.Change("Данные сохранены", MessageType.Ok);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _message.Change("Произошла ошибка, попробуйте позже", MessageType.Error);
        }
    }

    private Tuple<decimal, decimal> GetPrice()
    {
        decimal fullPrice = 0;
        decimal penalties = 0;
        if (_rent.Id != 0)
        {
            fullPrice = _rent.FinishPrice;
        }
        else
        {
            int days = (_rent.DateEnd.ToDateTime(TimeOnly.MinValue) - _rent.DateStart.ToDateTime(TimeOnly.MinValue))
                .Days;
            if (days > 0)
            {
                Auto auto = _cars.FirstOrDefault(c => c.Vin == _rent.AutoId);
                if (auto != null)
                {
                    fullPrice = auto.RentPrice * days;
                }
            }
        }

        penalties = (fullPrice / 100) * _rent.Penalties;

        return new Tuple<decimal, decimal>(fullPrice, penalties);
    }

    private void GenerateFile()
    {
        Document document = new Document();
        
        
        
        MemoryStream memoryStream = new MemoryStream();
        document.Save(memoryStream, SaveFormat.Docx);
    }

    private async Task DownloadFile()
    {
        byte[] fileContent;
        string fileName = $"{_rent.DateCreated}_{_rent.AutoId}-.docx";

        // await GetFile(fileContent, fileName);
    }

    private async Task GetFile(byte[] currentFileBytes, string currentFileName)
    {
        try
        {
            await JS.InvokeVoidAsync("downloadFile", currentFileName, currentFileBytes);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}