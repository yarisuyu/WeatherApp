using FluentAssertions;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Domain.UnitTests.Entities;

public class LocationTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesLocation()
    {
        var location = new Location("Moscow", 55.7558, 37.6173);

        location.City.Should().Be("Moscow");
        location.Latitude.Should().Be(55.7558);
        location.Longitude.Should().Be(37.6173);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenCityIsNullOrWhitespace_ThrowsArgumentException(string? invalidCity)
    {
        Action act = () => new Location(invalidCity!, 55.7558, 37.6173);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("city");
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(90.1)]
    [InlineData(180)]
    public void Constructor_WhenLatitudeOutOfRange_ThrowsArgumentOutOfRangeException(double invalidLatitude)
    {
        Action act = () => new Location("Moscow", invalidLatitude, 37.6173);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("latitude");
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(180.1)]
    [InlineData(360)]
    public void Constructor_WhenLongitudeOutOfRange_ThrowsArgumentOutOfRangeException(double invalidLongitude)
    {
        Action act = () => new Location("Moscow", 55.7558, invalidLongitude);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("longitude");
    }

    [Theory]
    [InlineData(-90, -180)]
    [InlineData(-90, 180)]
    [InlineData(90, -180)]
    [InlineData(90, 180)]
    public void Constructor_WithBoundaryValues_CreatesLocation(double latitude, double longitude)
    {
        var location = new Location("Moscow", latitude, longitude);

        location.Latitude.Should().Be(latitude);
        location.Longitude.Should().Be(longitude);
    }

    [Fact]
    public void Equality_WithSameValues_AreEqual()
    {
        var a = new Location("Moscow", 55.7558, 37.6173);
        var b = new Location("Moscow", 55.7558, 37.6173);

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}