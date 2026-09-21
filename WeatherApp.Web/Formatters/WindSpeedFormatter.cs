namespace WeatherApp.Web.Formatters;

public static class WindSpeedFormatter
{
    public static string ToWindDisplayMps(this double windSpeedMps)
        => $"{Math.Round(windSpeedMps, 1)} м/с";

    public static string ToWindDisplayKph(this double windSpeedKph)
        => $"{Math.Round(windSpeedKph, 1)} км/ч";
}