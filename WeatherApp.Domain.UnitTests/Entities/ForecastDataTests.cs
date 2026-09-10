using FluentAssertions;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.UnitTests.Entities;

public class ForecastDataTests
{
    private readonly Location _location = new("Moscow", 55.7558, 37.6173);
    private readonly List<HourlyForecast> _hourly;
    private readonly List<DailyForecast> _daily;

    public ForecastDataTests()
    {
        _hourly = new List<HourlyForecast>
        {
            new(new DateTime(2026, 9, 10, 10, 0, 0), Temperature.FromCelsius(18), 1000, "Sunny", WindSpeed.FromKph(5), 55),
            new(new DateTime(2026, 9, 10, 12, 0, 0), Temperature.FromCelsius(20), 1000, "Sunny", WindSpeed.FromKph(6), 50),
            new(new DateTime(2026, 9, 10, 14, 0, 0), Temperature.FromCelsius(22), 1000, "Sunny", WindSpeed.FromKph(7), 45),
        };

        _daily = new List<DailyForecast>
        {
            new(new DateTime(2026, 9, 10), Temperature.FromCelsius(25), Temperature.FromCelsius(15), Temperature.FromCelsius(20), 1000, "Sunny", 0),
            new(new DateTime(2026, 9, 11), Temperature.FromCelsius(23), Temperature.FromCelsius(14), Temperature.FromCelsius(18), 1003, "Cloudy", 1.5),
            new(new DateTime(2026, 9, 12), Temperature.FromCelsius(20), Temperature.FromCelsius(12), Temperature.FromCelsius(16), 1063, "Rain", 5.0),
        };
    }

    [Fact]
    public void Constructor_WithValidData_CreatesInstance()
    {
        var data = new ForecastData(_location, _hourly, _daily);

        data.Location.Should().Be(_location);
        data.HourlyForecasts.Should().HaveCount(3);
        data.DailyForecasts.Should().HaveCount(3);
    }

    [Fact]
    public void Constructor_WhenLocationNull_ThrowsArgumentNullException()
    {
        Action act = () => new ForecastData(null!, _hourly, _daily);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("location");
    }

    [Fact]
    public void Constructor_WhenHourlyNull_ThrowsArgumentNullException()
    {
        Action act = () => new ForecastData(_location, null!, _daily);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("hourlyForecasts");
    }

    [Fact]
    public void Constructor_WhenDailyNull_ThrowsArgumentNullException()
    {
        Action act = () => new ForecastData(_location, _hourly, null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dailyForecasts");
    }
}