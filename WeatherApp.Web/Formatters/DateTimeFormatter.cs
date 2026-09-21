using System.Globalization;

namespace WeatherApp.Web.Formatters;

public static class DateTimeFormatter
{
    public static string ToHourDisplay(this DateTime time) => time.ToString("HH:mm");

    public static string ToDayDisplay(this DateTime date)
    {
        var today = DateTime.Today;
        if (date.Date == today) return "Сегодня";
        if (date.Date == today.AddDays(1)) return "Завтра";
        return date.ToString("dddd, d MMMM", new System.Globalization.CultureInfo("ru-RU"));
    }

    public static string ToShortDate(this DateTime date)
        => date.ToString("d MMM", new System.Globalization.CultureInfo("ru-RU"));

    public static string ToDayTabLabel(this DateTime date)
    {
        var today = DateTime.Today;
        if (date.Date == today) return "Сегодня";
        if (date.Date == today.AddDays(1)) return "Завтра";
        return date.ToString("ddd, d", new CultureInfo("ru-RU"));
    }

}