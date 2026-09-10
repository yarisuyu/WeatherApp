using FluentAssertions;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.UnitTests.ValueObjects;

public class TemperatureTests
{
    [Fact]
    public void FromCelsius_WithValidValue_ShouldCreateInstance()
    {
        var temp = Temperature.FromCelsius(25.5);
        temp.Celsius.Should().Be(25.5);
    }

    [Fact]
    public void FromCelsius_WithAbsoluteZero_ShouldCreateInstance()
    {
        var temp = Temperature.FromCelsius(-273.15);
        temp.Celsius.Should().Be(-273.15);
    }

    [Fact]
    public void FromCelsius_BelowAbsoluteZero_ShouldThrowArgumentException()
    {
        Action act = () => Temperature.FromCelsius(-300);
        act.Should().Throw<ArgumentException>().WithMessage("*below absolute zero*");
    }
}