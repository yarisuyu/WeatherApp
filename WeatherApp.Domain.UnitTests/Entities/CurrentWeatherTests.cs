using FluentAssertions;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.UnitTests.Entities;

public class CurrentWeatherTests
{
    private readonly DateTime _lastUpdated = new(2026, 9, 10, 12, 0, 0);
    private readonly Temperature _temperature = Temperature.FromCelsius(22.5);
    private readonly Temperature _feelsLike = Temperature.FromCelsius(20.0);
    private readonly WindSpeed _wind = WindSpeed.FromKph(10.5);
    private const int ConditionCode = 1000;
    private const string ConditionText = "Sunny";
    private const int Humidity = 65;
    private const double UvIndex = 5.0;

    [Fact]
    public void Constructor_WithValidData_CreatesInstance()
    {
        var weather = new CurrentWeather(
            _lastUpdated, _temperature, ConditionCode, ConditionText,
            _wind, Humidity, _feelsLike, UvIndex);

        weather.LastUpdated.Should().Be(_lastUpdated);
        weather.Temperature.Should().Be(_temperature);
        weather.ConditionCode.Should().Be(ConditionCode);
        weather.ConditionText.Should().Be(ConditionText);
        weather.WindSpeed.Should().Be(_wind);
        weather.HumidityPercent.Should().Be(Humidity);
        weather.FeelsLike.Should().Be(_feelsLike);
        weather.UvIndex.Should().Be(UvIndex);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(200)]
    public void Constructor_WhenHumidityOutOfRange_ThrowsArgumentOutOfRangeException(int invalidHumidity)
    {
        Action act = () => new CurrentWeather(
            _lastUpdated, _temperature, ConditionCode, ConditionText,
            _wind, invalidHumidity, _feelsLike, UvIndex);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("humidityPercent");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Constructor_WithBoundaryHumidity_CreatesInstance(int humidity)
    {
        var weather = new CurrentWeather(
            _lastUpdated, _temperature, ConditionCode, ConditionText,
            _wind, humidity, _feelsLike, UvIndex);

        weather.HumidityPercent.Should().Be(humidity);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-5)]
    [InlineData(-100)]
    public void Constructor_WhenUvIndexNegative_ThrowsArgumentOutOfRangeException(double invalidUv)
    {
        Action act = () => new CurrentWeather(
            _lastUpdated, _temperature, ConditionCode, ConditionText,
            _wind, Humidity, _feelsLike, invalidUv);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("uvIndex");
    }

    [Fact]
    public void Constructor_WithZeroUvIndex_CreatesInstance()
    {
        var weather = new CurrentWeather(
            _lastUpdated, _temperature, ConditionCode, ConditionText,
            _wind, Humidity, _feelsLike, 0);

        weather.UvIndex.Should().Be(0);
    }
}