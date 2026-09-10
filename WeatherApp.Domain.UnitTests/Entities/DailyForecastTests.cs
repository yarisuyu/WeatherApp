using FluentAssertions;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.UnitTests.Entities;

public class DailyForecastTests
{
    private readonly DateTime _date = new(2026, 9, 10);
    private readonly Temperature _max = Temperature.FromCelsius(25.0);
    private readonly Temperature _min = Temperature.FromCelsius(15.0);
    private readonly Temperature _avg = Temperature.FromCelsius(20.0);
    private const int ConditionCode = 1000;
    private const string ConditionText = "Sunny";
    private const double Precipitation = 0.5;

    [Fact]
    public void Constructor_WithValidData_CreatesInstance()
    {
        var forecast = new DailyForecast(
            _date, _max, _min, _avg, ConditionCode, ConditionText, Precipitation);

        forecast.Date.Should().Be(_date);
        forecast.MaxTemperature.Should().Be(_max);
        forecast.MinTemperature.Should().Be(_min);
        forecast.AvgTemperature.Should().Be(_avg);
        forecast.ConditionCode.Should().Be(ConditionCode);
        forecast.ConditionText.Should().Be(ConditionText);
        forecast.TotalPrecipitationMm.Should().Be(Precipitation);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-5)]
    [InlineData(-100)]
    public void Constructor_WhenPrecipitationNegative_ThrowsArgumentOutOfRangeException(double invalidPrecip)
    {
        Action act = () => new DailyForecast(
            _date, _max, _min, _avg, ConditionCode, ConditionText, invalidPrecip);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("totalPrecipitationMm");
    }

    [Fact]
    public void Constructor_WithZeroPrecipitation_CreatesInstance()
    {
        var forecast = new DailyForecast(
            _date, _max, _min, _avg, ConditionCode, ConditionText, 0);

        forecast.TotalPrecipitationMm.Should().Be(0);
    }
}