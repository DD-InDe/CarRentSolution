using CarRentSolution.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CarRentSolution.Components.Pages;

public partial class ClientInfo : ComponentBase
{
    [Parameter] public long? Id { get; set; }
    [Parameter] public bool? IsView { get; set; }
    [Parameter] public string? FullName { get; set; }
    [Parameter] public string? Phone { get; set; }

    private bool _isView => IsView ?? false;
    private bool _isCreate;
    private bool _isLoaded = false;

    private List<Order> _orders;
    private Tenant _tenant;

    protected override async Task OnInitializedAsync()
    {
        _isCreate = Id == null;

        if (_isCreate)
        {
            _tenant = new();
            if (FullName != null && Phone != null)
            {
                string[] name = FullName.Split(" ");
                _tenant.LastName = name[0];
                _tenant.FirstName = name[1];
                if (name.Length == 3) _tenant.MiddleName = name[2];
                _tenant.Phone = Phone;
            }
        }
        else
            _tenant = await Db
                .Tenants.Include(c => c.TenantDocument)
                .FirstOrDefaultAsync(c => c.Id == Id);

        if (_tenant == null)
        {
            Navigation.NavigateTo("/");
            return;
        }

        if (_tenant.TenantDocument == null) _tenant.TenantDocument = new();

        _isLoaded = true;
    }

    private async Task SaveClient()
    {
        try
        {
            if (_tenant.Id == 0)
            {
                await Db.Tenants.AddAsync(_tenant);
                await Db.TenantDocuments.AddAsync(_tenant.TenantDocument);
                Navigation.NavigateTo("/clients");
            }
            else
            {
                Db.Tenants.Update(_tenant);
                if (_tenant.TenantDocument.Id == 0) await Db.TenantDocuments.AddAsync(_tenant.TenantDocument);
                else Db.TenantDocuments.Update(_tenant.TenantDocument);
            }

            await Db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task SaveFiles()
    {
        if (_tenant.TenantDocument.Passport != null && _tenant.TenantDocument.DriveLicense != null)
        {
            if (_tenant.TenantDocument.Id == 0)
                await Db.TenantDocuments.AddAsync(_tenant.TenantDocument);
            else
                Db.TenantDocuments.Update(_tenant.TenantDocument);

            await Db.SaveChangesAsync();
        }
    }

    private async Task LoadPassport(InputFileChangeEventArgs obj)
    {
        _tenant.TenantDocument.Passport = await LoadFile(obj);
    }

    private async Task LoadLicense(InputFileChangeEventArgs obj)
    {
        _tenant.TenantDocument.DriveLicense = await LoadFile(obj);
    }

    private async Task<string?> LoadFile(InputFileChangeEventArgs obj)
    {
        try
        {
            long maxSize = 20000000; // максимальный размер файла (20 мб)

            if (obj.File.Size > maxSize) // проверка размера
            {
                // todo: добавить сообщение на странице
                return null;
            }

            MemoryStream memoryStream = new MemoryStream();
            Stream stream = obj.File.OpenReadStream(maxSize); // открываем для чтения
            await stream.CopyToAsync(memoryStream);
            stream.Close(); // за
            return Convert.ToBase64String(memoryStream.ToArray());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
        finally
        {
            obj
                .File.OpenReadStream()
                .Close();
        }
    }

    private async Task DownloadFile(int choice)
    {
        byte[] fileContent;
        string fileName = $"{_tenant.LastName}_-.pdf";
        if (choice == 1)
        {
            fileContent = Convert.FromBase64String(_tenant.TenantDocument.Passport);
            fileName = fileName.Replace("-", "паспорт");
        }
        else
        {
            fileContent = Convert.FromBase64String(_tenant.TenantDocument.DriveLicense);
            fileName = fileName.Replace("-", "права");
        }

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

    private void OpenEdit()
    {
        Navigation.NavigateTo($"client/{Id}/false");
    }
}