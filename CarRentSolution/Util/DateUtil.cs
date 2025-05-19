namespace CarRentSolution.Util;

public abstract class DateUtil
{
    public static string GetMonthName(int month)
    {
        return month switch
        {
            1 => "Января",
            2 => "Февраля",
            3 => "Марта",
            4 => "Апреля",
            5 => "Мая",
            6 => "Июня",
            7 => "Июля",
            8 => "Августа",
            9 => "Сентября",
            10 => "Октября",
            11 => "Ноября",
            12 => "Декабря",
            _ => "-"
        };
    }
}