using FluentAssertions;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.UnitTests.Entities;

public class HourlyForecastTests
{
    private readonly DateTime _time = new(2026, 9, 10, 14, 0, 0);
    private readonly Temperature _temperature = Temperature.FromCelsius(18.5);
    private readonly WindSpeed _wind = WindSpeed.FromKph(12.0);
    private const int ConditionCode = 1003;
    private const string ConditionText = "Partly cloudy";
    private const int Humidity = 55;

    [Fact]
    public void Constructor_WithValidData_CreatesInstance()
    {
        var forecast = new HourlyForecast(
            _time, _temperature, ConditionCode, ConditionText, _wind, Humidity);

        forecast.Time.Should().Be(_time);
        forecast.Temperature.Should().Be(_temperature);
        forecast.ConditionCode.Should().Be(ConditionCode);
        forecast.ConditionText.Should().Be(ConditionText);
        forecast.WindSpeed.Should().Be(_wind);
        forecast.HumidityPercent.Should().Be(Humidity);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_WhenHumidityOutOfRange_ThrowsArgumentOutOfRangeException(int invalidHumidity)
    {
        Action act = () => new HourlyForecast(
            _time, _temperature, ConditionCode, ConditionText, _wind, invalidHumidity);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("humidityPercent");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void Constructor_WithValidHumidity_CreatesInstance(int humidity)
    {
        var forecast = new HourlyForecast(
            _time, _temperature, ConditionCode, ConditionText, _wind, humidity);

        forecast.HumidityPercent.Should().Be(humidity);
    }

    [Fact]
    public void Constructor_WithEmptyConditionText_CreatesInstance()
    {
        var forecast = new HourlyForecast(
            _time, _temperature, ConditionCode, string.Empty, _wind, Humidity);

        forecast.ConditionText.Should().BeEmpty();
    }
}