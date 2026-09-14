using ErrorOr;
using FluentAssertions;
using Mapster;
using MapsterMapper;
using Moq;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.Mappings;
using WeatherApp.Application.Queries;
using WeatherApp.Application.UnitTests.TestData;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.UnitTests.Queries;

public class GetCurrentWeatherQueryHandlerTests
{
    private readonly double _lat = WeatherTestDataFactory.DefaultLatitude;
    private readonly double _lon = WeatherTestDataFactory.DefaultLongitude;
    private readonly Mock<IWeatherDataProvider> _providerMock;
    private readonly IMapper _mapper;
    private readonly GetWeatherQueryHandler _handler;

    public GetCurrentWeatherQueryHandlerTests()
    {
        _providerMock = new Mock<IWeatherDataProvider>();

        var config = new TypeAdapterConfig();
        config.Apply(new WeatherMappingProfile());
        _mapper = new Mapper(config);

        _handler = new GetWeatherQueryHandler(_providerMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WhenDataProviderReturnsData_ShouldReturnSuccessResult()
    {
        // Arrange
        var currentWeatherData = WeatherTestDataFactory.CreateWeatherData();
        _providerMock
            .Setup(p => p.GetCurrentWeatherAsync(_lat, _lon, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentWeatherData);

        var query = new GetCurrentWeatherQuery(_lat, _lon);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.CurrentWeather.TemperatureCelsius.Should().Be(currentWeatherData.Current.Temperature.Celsius);
    }

    [Fact]
    public async Task Handle_WhenDataProviderThrowsException_ShouldReturnError()
    {
        // Arrange
        _providerMock
            .Setup(p => p.GetCurrentWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Network error"));

        var query = new GetCurrentWeatherQuery(_lat, _lon);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public async Task Handle_WhenDataProviderReturnsNull_ShouldReturnError()
    {
        // Arrange
        _providerMock.Setup(p => p.GetCurrentWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherData)null!);

        var query = new GetCurrentWeatherQuery(_lat, _lon);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
    }
}