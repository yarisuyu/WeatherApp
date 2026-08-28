using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Options;
using WeatherApp.Infrastructure.Options;
using WeatherApp.Infrastructure.Services;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WeatherApp.Infrastructure.IntegrationTests.Services;

public class WeatherApiClientTests : IDisposable
{
    private readonly WireMockServer _server;
    private readonly HttpClient _httpClient;
    private readonly WeatherApiOptions _options;
    private readonly WeatherApiClient _sut;

    public WeatherApiClientTests()
    {
        _server = WireMockServer.Start();
        _options = new WeatherApiOptions
        {
            BaseUrl = _server.Url!,
            ApiKey = "test-key"
        };

        _httpClient = new HttpClient { BaseAddress = new Uri(_server.Url!) };
        _sut = new WeatherApiClient(
            _httpClient,
            new OptionsWrapper<WeatherApiOptions>(_options));
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _server.Stop();
        _server.Dispose();
    }

    // ============ 1. Культура: координаты через точку ============

    [Fact]
    public async Task GetCurrentWeatherAsync_UsesInvariantCulture_ForCoordinates()
    {
        // Arrange
        var lat = 55.7558;
        var lon = 37.6173;

        _server.Given(Request.Create().WithPath("/current.json").UsingGet())
            .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(CurrentWeatherJson));

        // Act
        await _sut.GetCurrentWeatherAsync(lat, lon);

        // Assert
        var request = _server.LogEntries.Single().RequestMessage;

        // Ключевое: точка, а не запятая
        request?.RawQuery.Should().Contain("55.7558");
        request?.RawQuery.Should().NotContain("55,7558");
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_FormatsCoordinates_RegardlessOfCurrentCulture()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            // Эмулируем русскую локаль — десятичный разделитель запятая
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");

            _server.Given(Request.Create().WithPath("/current.json").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(CurrentWeatherJson));

            // Act
            await _sut.GetCurrentWeatherAsync(55.7558, 37.6173);

            // Assert
            var entry = _server.LogEntries.Single();
            entry.RequestMessage!.RawQuery.Should().Be("?key=test-key&q=55.7558,37.6173");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    // ============ 2. URL содержит key, q, days ============

    [Fact]
    public async Task GetForecastAsync_BuildsUrlWithApiKeyCoordinatesAndDays()
    {
        // Arrange
        _server.Given(Request.Create().WithPath("/forecast.json").UsingGet())
            .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(ForecastJson));

        // Act
        await _sut.GetForecastAsync(55.7558, 37.6173, 3);

        // Assert
        var request = _server.LogEntries.Single().RequestMessage;
        var query = request?.Query!;

        query["key"].Single().Should().Be("test-key");
        query["q"].First().Should().Be("55.7558");
        query["q"].Last().Should().Be("37.6173");
        query["days"].Single().Should().Be("3");
    }

    // ============ 3. Десериализация в Domain ============

    [Fact]
    public async Task GetCurrentWeatherAsync_DeserializesJson_IntoDomainEntity()
    {
        // Arrange
        _server.Given(Request.Create().WithPath("/current.json").UsingGet())
            .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(CurrentWeatherJson));

        // Act
        var result = await _sut.GetCurrentWeatherAsync(55.7558, 37.6173);
        // Assert
        result.Location.City.Should().Be("Moscow");
        result.Current.Temperature.Celsius.Should().Be(22.5);
        result.Current.ConditionCode.Should().Be(1000);
        result.Current.ConditionIcon.Should().Be("https://example.com/sunny.png");
        result.Current.ConditionText.Should().Be("Sunny");
        result.Current.HumidityPercent.Should().Be(65);
    }

    [Fact]
    public async Task GetForecastAsync_DeserializesJson_IntoDomainEntity()
    {
        // Arrange
        _server.Given(Request.Create().WithPath("/forecast.json").UsingGet())
            .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(ForecastJson));

        // Act
        var result = await _sut.GetForecastAsync(55.7558, 37.6173, 3);

        // Assert
        result.Location.City.Should().Be("Moscow");
        result.HourlyForecasts.Should().NotBeEmpty();
        result.DailyForecasts.Should().HaveCount(3);
    }

    // ============ 4. Обработка ошибок HTTP ============

    [Fact]
    public async Task GetCurrentWeatherAsync_WhenApiReturns404_ThrowsHttpRequestException()
    {
        // Arrange
        _server.Given(Request.Create().WithPath("/current.json").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(404));

        // Act
        var act = async () => await _sut.GetCurrentWeatherAsync(55.7558, 37.6173);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_WhenApiReturns500_ThrowsHttpRequestException()
    {
        // Arrange
        _server.Given(Request.Create().WithPath("/current.json").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(500));

        // Act
        var act = async () => await _sut.GetCurrentWeatherAsync(55.7558, 37.6173);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_WhenJsonIsMalformed_Throws()
    {
        // Arrange
        _server.Given(Request.Create().WithPath("/current.json").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("{ not valid json }"));

        // Act
        var act = async () => await _sut.GetCurrentWeatherAsync(55.7558, 37.6173);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    // ============ JSON-заглушки ============

    private const string CurrentWeatherJson = """
    {
        "location": {
            "name": "Moscow",
            "lat": 55.7558,
            "lon": 37.6173
        },
        "current": {
            "last_updated": "2026-09-16 12:00",
            "temp_c": 22.5,
            "condition": { "code": 1000, "icon": "https://example.com/sunny.png", "text": "Sunny" },
            "wind_kph": 10.5,
            "humidity": 65,
            "feelslike_c": 20.0,
            "uv": 5.0
        }
    }
    """;

    private const string ForecastJson = """
    {
        "location": {
            "name": "Moscow",
            "lat": 55.7558,
            "lon": 37.6173
        },
        "forecast": {
            "forecastday": [
                {
                    "date": "2026-09-16",
                    "day": {
                        "maxtemp_c": 25.0,
                        "mintemp_c": 15.0,
                        "avgtemp_c": 20.0,
                        "condition": { "code": 1000, "icon": "https://example.com/sunny.png", "text": "Sunny" },
                        "totalprecip_mm": 0.0
                    },
                    "hour": [
                        {
                            "time": "2026-09-16 12:00",
                            "temp_c": 22.0,
                            "condition": { "code": 1000, "icon": "https://example.com/sunny.png", "text": "Sunny" },
                            "wind_kph": 10.0,
                            "humidity": 60
                        }
                    ]
                },
                {
                    "date": "2026-09-17",
                    "day": {
                        "maxtemp_c": 23.0,
                        "mintemp_c": 14.0,
                        "avgtemp_c": 18.0,
                        "condition": { "code": 1003, "icon": "https://example.com/sunny.png", "text": "Partly cloudy" },
                        "totalprecip_mm": 1.5
                    },
                    "hour": []
                },
                {
                    "date": "2026-09-18",
                    "day": {
                        "maxtemp_c": 20.0,
                        "mintemp_c": 12.0,
                        "avgtemp_c": 16.0,
                        "condition": { "code": 1063, "icon": "https://example.com/sunny.png", "text": "Rain" },
                        "totalprecip_mm": 5.0
                    },
                    "hour": []
                }
            ]
        }
    }
    """;
}