using CarRentSolution.Entity;
using CarRentSolution.PageModel;
using CarRentSolution.Util;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Xceed.Words.NET;

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
                    .Include(c => c.Auto)
                    .Include(c => c.Auto.Model)
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

    private async Task<byte[]> GenerateFile()
    {
        var document = DocX.Load("Documents/template.docx");
        try
        {
            Company company = (await Db.Companies.ToListAsync()).Last();

            Tuple<string, string>[] values = new[]
            {
                new Tuple<string, string>("day_create", _rent.DateCreated.Day.ToString()),
                new Tuple<string, string>("month_create", DateUtil.GetMonthName(_rent.DateCreated.Month).ToLower()),
                new Tuple<string, string>("year_create", _rent.DateCreated.Year.ToString()),
                new Tuple<string, string>("company_name", company.Name),
                new Tuple<string, string>("company_boss", company.Boss),
                new Tuple<string, string>("document_boss", company.Document),
                new Tuple<string, string>("client_name", _rent.Tenant.FullName),
                new Tuple<string, string>("client_pass_s", _rent.Tenant.PassportSerial),
                new Tuple<string, string>("client_pass_n", _rent.Tenant.PassportNumber),
                new Tuple<string, string>("client_pass_by", _rent.Tenant.PassportIssued),
                new Tuple<string, string>("client_address", _rent.Tenant.Address),
                new Tuple<string, string>("auto_model", _rent.Auto.Model.Name),
                new Tuple<string, string>("auto_year", _rent.Auto.Year.ToString()),
                new Tuple<string, string>("auto_vin", _rent.AutoId),
                new Tuple<string, string>("auto_engine", _rent.Auto.Engine),
                new Tuple<string, string>("auto_body", _rent.Auto.Body),
                new Tuple<string, string>("auto_color", _rent.Auto.Color.ToLower()),
                new Tuple<string, string>("auto_number", _rent.Auto.GovNumber),
                new Tuple<string, string>("auto_pass_n", _rent.Auto.Passport),
                new Tuple<string, string>("auto_pass_by", _rent.Auto.PassportIssued ?? "-"),
                new Tuple<string, string>("auto_pass_date", _rent.Auto.PassportDated.ToString()),
                new Tuple<string, string>("rent_fix", _rent.RepairCondition.Name),
                new Tuple<string, string>("rent_price", _rent.FinishPrice.ToString()),
                new Tuple<string, string>("rent_start_day", _rent.DateStart.Day.ToString()),
                new Tuple<string, string>("rent_start_month", DateUtil.GetMonthName(_rent.DateStart.Month)),
                new Tuple<string, string>("rent_start_year", _rent.DateStart.Year.ToString()),
                new Tuple<string, string>("rent_end_day", _rent.DateEnd.Day.ToString()),
                new Tuple<string, string>("rent_end_month", DateUtil.GetMonthName(_rent.DateEnd.Month)),
                new Tuple<string, string>("rent_end_year", _rent.DateEnd.Year.ToString()),
                new Tuple<string, string>("rent_peny", _rent.Penalties.ToString()),
                new Tuple<string, string>("auto_fullprice", _rent.Auto.FullPrice.ToString()),
                new Tuple<string, string>("company_address", company.LegalAddress),
                new Tuple<string, string>("company_email", company.Email),
                new Tuple<string, string>("company_inn", company.Inn),
                new Tuple<string, string>("company_kpp", company.Kpp),
                new Tuple<string, string>("company_r", company.CurrentAccount),
                new Tuple<string, string>("company_k", company.CorrespondentAccount),
                new Tuple<string, string>("company_bik", company.Bik),
                new Tuple<string, string>("client_phone", _rent.Tenant.Phone),
                new Tuple<string, string>("client_email", _rent.Tenant.Email ?? "-"),
                new Tuple<string, string>("client_pass_date", _rent.Tenant.PassportDated.ToString()),
                new Tuple<string, string>("client_pass_s2", _rent.Tenant.PassportSerial),
                new Tuple<string, string>("client_pass_n2", _rent.Tenant.PassportNumber),
                new Tuple<string, string>("client_pass_by2", _rent.Tenant.PassportIssued),
                new Tuple<string, string>("client_address2", _rent.Tenant.Address)
            };

            foreach (var value in values)
            {
                Console.WriteLine($"{value.Item1} - {value.Item2}");
                var bookmark = document.Bookmarks[value.Item1];
                bookmark.SetText(value.Item2);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        MemoryStream memoryStream = new MemoryStream();
        document.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    private async Task DownloadFile()
    {
        byte[] fileContent = await GenerateFile();
        string fileName = $"{_rent.DateCreated}_{_rent.AutoId}.docx";

        await GetFile(fileContent, fileName);
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