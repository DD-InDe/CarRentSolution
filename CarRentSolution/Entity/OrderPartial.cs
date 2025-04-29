using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CarRentSolution.Entity;

public partial class Order
{
    [NotMapped] public string? ClientFullName { get; set; }
    [NotMapped]
    public string? ClientPreviewPhone
    {
        get { return FormatPhoneNumber(ClientPhone); }
        set
        {
            // Удаляем все символы, кроме цифр
            string phoneNumber = new string((value ?? "").Where(char.IsDigit).ToArray());

            // Форматируем номер
            if (phoneNumber.Length > 10)
            {
                phoneNumber = phoneNumber.Substring(0, 10);
            }
            string formattedPhoneNumber = Regex.Replace(phoneNumber, @"(\d{1})(\d{3})(\d{3})(\d{2})(\d{2})", "+7($2)$3-$4-$5");

            // Обновляем значение свойства
            ClientPhone = formattedPhoneNumber;
        }
    }

    private string FormatPhoneNumber(string? phoneNumber)
    {
        // Удаляем все символы, кроме цифр
        string digits = new string((phoneNumber ?? "").Where(char.IsDigit).ToArray());

        // Форматируем номер
        if (digits.Length > 10)
        {
            digits = digits.Substring(0, 10);
        }
        return Regex.Replace(digits, @"(\d{1})(\d{3})(\d{3})(\d{2})(\d{2})", "+7($2)$3-$4-$5");
    }

}