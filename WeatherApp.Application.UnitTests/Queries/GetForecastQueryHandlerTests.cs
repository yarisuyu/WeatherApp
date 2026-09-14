using FluentAssertions;
using Mapster;
using MapsterMapper;
using Moq;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.Mappings;
using WeatherApp.Application.Queries;
using WeatherApp.Application.UnitTests.TestData;
using WeatherApp.Domain.Entities;
using Microsoft.Extensions.Time.Testing;

namespace WeatherApp.Application.UnitTests.Queries;

public class GetForecastQueryHandlerTests
{
    private readonly Location _location = WeatherTestDataFactory.CreateLocation();
    private readonly Mock<IWeatherDataProvider> _mockProvider;
    private readonly TypeAdapterConfig _config;
    private readonly IMapper _mapper;
    private readonly GetForecastQueryHandler _handler;

    public GetForecastQueryHandlerTests()
    {
        _mockProvider = new Mock<IWeatherDataProvider>();

        _config = new TypeAdapterConfig();
        _config.Apply(new WeatherMappingProfile()); // регистрируем профиль
        _mapper = new Mapper(_config);
        
        _handler = new GetForecastQueryHandler(_mockProvider.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WhenDailyForecast_ShouldReturnHourlyAndDailyForecasts()
    {
        // Arrange
        var now = new DateTimeOffset(2026, 9, 10, 12, 0, 0, TimeSpan.FromHours(3));
        var fakeTime = new FakeTimeProvider(now); // задаём локальное время

        var forecastData = WeatherTestDataFactory.CreateForecastData();
        _mockProvider.Setup(p => p.GetForecastAsync(
            _location.Latitude, 
            _location.Longitude, 
            3, 
            It.IsAny<CancellationToken>()
        ))
            .ReturnsAsync(forecastData);

        var query = new GetForecastQuery(_location.Latitude, _location.Longitude, 3);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.HourlyForecasts.Should().HaveCount(24); 
        result.Value.DailyForecasts.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WhenProviderThrows_ShouldReturnError()
    {
        // Arrange
        _mockProvider.Setup(p => p.GetForecastAsync(
            It.IsAny<double>(),
            It.IsAny<double>(),
            3,
            It.IsAny<CancellationToken>()
        ))
            .ThrowsAsync(new Exception("Service unavailable"));

        var query = new GetForecastQuery(_location.Latitude, _location.Longitude, 3);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenForecastEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        var empty = new ForecastData(_location, [], []);
        _mockProvider.Setup(p => p.GetForecastAsync(
            _location.Latitude, 
            _location.Longitude, 
            3, 
            It.IsAny<CancellationToken>()
        ))
            .ReturnsAsync(empty);

        var query = new GetForecastQuery(_location.Latitude, _location.Longitude, 3);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Value.HourlyForecasts.Should().BeEmpty();
        result.Value.DailyForecasts.Should().BeEmpty();
    }
}