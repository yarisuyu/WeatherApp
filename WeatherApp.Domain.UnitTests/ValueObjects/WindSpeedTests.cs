using FluentAssertions;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.UnitTests.ValueObjects;

public class WindSpeedTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(10.5)]
    [InlineData(123.456)]
    public void FromKph_WithValidValue_ShouldCreateInstance(double kph)
    {
        // Act
        var wind = WindSpeed.FromKph(kph);

        // Assert
        wind.Kph.Should().Be(kph);
        wind.Mps.Should().Be(kph / 3.6, because: "1 m/s = 3.6 km/h");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2.5)]
    [InlineData(100.0)]
    public void FromMps_WithValidValue_ShouldCreateInstance(double mps)
    {
        // Act
        var wind = WindSpeed.FromMps(mps);

        // Assert
        wind.Mps.Should().Be(mps);
        wind.Kph.Should().Be(mps * 3.6, because: "1 m/s = 3.6 km/h");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-5)]
    [InlineData(-999.9)]
    public void FromKph_WithNegativeValue_ShouldThrowArgumentException(double negativeKph)
    {
        // Act
        Action act = () => WindSpeed.FromKph(negativeKph);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*negative*", because: "скорость ветра не может быть отрицательной");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-1.5)]
    [InlineData(-100)]
    public void FromMps_WithNegativeValue_ShouldThrowArgumentException(double negativeMps)
    {
        // Act
        Action act = () => WindSpeed.FromMps(negativeMps);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*negative*", because: "скорость ветра не может быть отрицательной");
    }

    [Fact]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var wind1 = WindSpeed.FromKph(15.0);
        var wind2 = WindSpeed.FromKph(15.0);

        // Assert
        wind1.Should().Be(wind2);
        (wind1 == wind2).Should().BeTrue();
        wind1.GetHashCode().Should().Be(wind2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var wind1 = WindSpeed.FromKph(15.0);
        var wind2 = WindSpeed.FromKph(20.0);

        // Assert
        wind1.Should().NotBe(wind2);
        (wind1 == wind2).Should().BeFalse();
    }

    [Fact]
    public void FromKph_AndFromMps_WithSameSpeed_ShouldBeEqual()
    {
        // Arrange
        var fromKph = WindSpeed.FromKph(36.0);
        var fromMps = WindSpeed.FromMps(10.0);

        // Assert
        fromKph.Should().Be(fromMps);
        fromKph.Kph.Should().Be(36.0);
        fromMps.Mps.Should().Be(10.0);
    }
}