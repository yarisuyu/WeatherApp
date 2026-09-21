namespace WeatherApp.Web.Formatters;

public static class TemperatureFormatter
{
    public static string ToTemperatureDisplay(this double celsius)
    {
        var rounded = Math.Round(celsius, 1);
        var sign = rounded > 0 ? "+" : string.Empty;
        return $"{sign}{rounded}°C";
    }

    public static string ToTemperatureDisplayShort(this double celsius)
        => $"{Math.Round(celsius)}°";
}