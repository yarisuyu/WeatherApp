using FluentAssertions;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.UnitTests.Entities;

public class WeatherDataTests
{
    private readonly Location _location = new("Moscow", 55.7558, 37.6173);
    private readonly CurrentWeather _current;
    public WeatherDataTests()
    {
        _current = new CurrentWeather(
            DateTime.UtcNow,
            Temperature.FromCelsius(20),
            1000, "Sunny",
            WindSpeed.FromKph(10),
            60,
            Temperature.FromCelsius(18),
            3);
    }

    [Fact]
    public void Constructor_WithValidData_CreatesInstance()
    {
        var data = new WeatherData(_location, _current);

        data.Location.Should().Be(_location);
        data.Current.Should().Be(_current);
    }

    [Fact]
    public void Constructor_WhenLocationNull_ThrowsArgumentNullException()
    {
        Action act = () => new WeatherData(null!, _current);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("location");
    }

    [Fact]
    public void Constructor_WhenCurrentNull_ThrowsArgumentNullException()
    {
        Action act = () => new WeatherData(_location, null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("current");
    }
}