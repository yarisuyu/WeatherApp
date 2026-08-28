using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System.Globalization;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.Services;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Infrastructure.IntegrationTests.Services;

public class WeatherDataProviderTests : IDisposable
{
    private readonly Mock<IWeatherApiClient> _apiMock;
    private readonly IMemoryCache _cache;
    private readonly FakeTimeProvider _timeProvider;

    private const double LatMoscow = 55.7558;
    private const double LonMoscow = 37.6173;
    private const double LatSpb = 59.9343;
    private const double LonSpb = 30.3351;


    public WeatherDataProviderTests()
    {
        _apiMock = new Mock<IWeatherApiClient>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _timeProvider = new FakeTimeProvider();
    }

    public void Dispose() {
        _cache.Dispose();
        GC.SuppressFinalize(this); 
    }

    private WeatherDataProvider CreateSut(
    TimeSpan? weatherTtl = null,
    TimeSpan? forecastTtl = null,
    IMemoryCache? cache = null,
    FakeTimeProvider? timeProvider = null)
    {
        var options = Microsoft.Extensions.Options.Options.Create(new WeatherDataProviderOptions
        {
            CurrentWeatherCacheTtl = weatherTtl ?? TimeSpan.FromMinutes(5),
            ForecastCacheTtl = forecastTtl ?? TimeSpan.FromMinutes(60)
        });

        return new WeatherDataProvider(
            _apiMock.Object,
            cache ?? _cache,
            NullLogger<WeatherDataProvider>.Instance,
            timeProvider ?? _timeProvider,
            options);
    }

    // Проверяет, что при первом вызове данные запрашиваются из API,
    // возвращаются как есть и API вызывается ровно один раз.

    [Fact]
    public async Task GetCurrentWeatherAsync_FirstCall_HitsApi()
    {
        // Arrange
        var weatherData = CreateDefaultWeatherData();
        SetupCurrentWeather(weatherData);
        var sut = CreateSut();

        // Act
        var result = await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);

        // Assert
        result.Should().BeEquivalentTo(weatherData);
        _apiMock.Verify(x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Проверяет, что второй вызов с теми же координатами возвращает
    // тот же экземпляр из кэша и не обращается к API повторно.

    [Fact]
    public async Task GetCurrentWeatherAsync_SecondCall_ReturnsFromCache()
    {
        // Arrange
        var weatherData = CreateDefaultWeatherData();
        SetupCurrentWeather(weatherData);
        var sut = CreateSut();

        // Act
        var first = await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);
        var second = await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);

        // Assert
        first.Should().BeSameAs(second);
        _apiMock.Verify(x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Проверяет, что разные координаты используют разные ключи кэша
    // и каждый город запрашивается из API отдельно.

    [Fact]
    public async Task GetCurrentWeatherAsync_DifferentCoordinates_UseSeparateCacheKeys()
    {
        // Arrange
        var moscow = CreateDefaultWeatherData();
        var spb = CreateWeatherData("Saint Petersburg", LatSpb, LonSpb);

        SetupCurrentWeather(moscow);
        SetupCurrentWeather(spb);
        var sut = CreateSut();

        // Act
        var resultMoscow = await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);
        var resultSpb = await sut.GetCurrentWeatherAsync(LatSpb, LonSpb);

        // Assert
        resultMoscow.Should().BeSameAs(moscow);
        resultSpb.Should().BeSameAs(spb);
        _apiMock.Verify(x => x.GetCurrentWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    // Проверяет, что прогноз и текущая погода кэшируются под разными ключами
    // и не перетирают друг друга.

    [Fact]
    public async Task GetForecastAsync_UsesSeparateCacheFromWeather()
    {
        // Arrange
        var weather = CreateDefaultWeatherData();
        var forecast = CreateForecastData();

        SetupCurrentWeather(weather);
        SetupForecast(forecast, 3);
        var sut = CreateSut();

        // Act
        await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);
        await sut.GetForecastAsync(LatMoscow, LonMoscow, 3);

        // Assert
        _apiMock.Verify(x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()), Times.Once);
        _apiMock.Verify(x => x.GetForecastAsync(LatMoscow, LonMoscow, 3, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Проверяет, что после истечения TTL для текущей погоды
    // следующий вызов снова идёт в API.

    [Fact]
    public async Task GetCurrentWeatherAsync_WhenCacheExpires_HitsApiAgain()
    {
        // Arrange
        var weatherData = CreateDefaultWeatherData();
        SetupCurrentWeather(weatherData);
        var sut = CreateSut(weatherTtl: TimeSpan.FromMilliseconds(50));

        // Act
        await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);
        _timeProvider.Advance(TimeSpan.FromMilliseconds(100));
        await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);

        // Assert
        _apiMock.Verify(x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    // Проверяет, что повторный запрос прогноза с тем же dayCount
    // возвращается из кэша без повторного обращения к API.

    [Fact]
    public async Task GetForecastAsync_SecondCall_ReturnsFromCache()
    {
        // Arrange
        var forecast = CreateForecastData();
        SetupForecast(forecast, 3);

        var sut = CreateSut();

        // Act  
        var first = await sut.GetForecastAsync(LatMoscow, LonMoscow, 3);
        var second = await sut.GetForecastAsync(LatMoscow, LonMoscow, 3);

        // Assert
        first.Should().BeSameAs(second);
        _apiMock.Verify(x => x.GetForecastAsync(LatMoscow, LonMoscow, 3, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Проверяет, что прогнозы с разным dayCount кэшируются отдельно
    // и каждый запрашивается из API.

    [Fact]
    public async Task GetForecastAsync_DifferentDayCount_UseSeparateCacheKeys()
    {
        // Arrange
        var forecast1 = CreateForecastData();
        var forecast5 = CreateForecastData();

        SetupForecast(forecast1, 1);
        SetupForecast(forecast5, 5);

        var sut = CreateSut();

        // Act
        var result1 = await sut.GetForecastAsync(LatMoscow, LonMoscow, 1);
        var result5 = await sut.GetForecastAsync(LatMoscow, LonMoscow, 5);

        // Assert
        result1.Should().BeSameAs(forecast1);
        result5.Should().BeSameAs(forecast5);
        _apiMock.Verify(x => x.GetForecastAsync(LatMoscow, LonMoscow, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    // Проверяет, что после истечения TTL для прогноза
    // следующий вызов снова идёт в API.

    [Fact]
    public async Task GetForecastAsync_WhenCacheExpires_HitsApiAgain()
    {
        // Arrange
        var forecast = CreateForecastData();
        SetupForecast(forecast, 3);

        var sut = CreateSut(forecastTtl: TimeSpan.FromMilliseconds(50));

        // Act
        await sut.GetForecastAsync(LatMoscow, LonMoscow, 3);
        _timeProvider.Advance(TimeSpan.FromMilliseconds(100));
        await sut.GetForecastAsync(LatMoscow, LonMoscow, 3);

        // Assert
        _apiMock.Verify(x => x.GetForecastAsync(LatMoscow, LonMoscow, 3, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    // Проверяет, что ключ кэша не зависит от текущей культуры:
    // при ru-RU и InvariantCulture второй вызов берётся из кэша,
    // API вызывается один раз.

    [Fact]
    public async Task GetCurrentWeatherAsync_CacheKey_IsCultureInvariant()
    {
        // Arrange
        var weatherData = CreateDefaultWeatherData();
        SetupCurrentWeather(weatherData);

        var sut = CreateSut();
        var original = CultureInfo.CurrentCulture;

        try
        {
            // Act
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);

            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }

        // Assert
        _apiMock.Verify(x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Проверяет, что если API бросает OperationCanceledException,
    // провайдер пробрасывает его наружу.

    [Fact]
    public async Task GetCurrentWeatherAsync_WhenApiThrowsCancellation_Propagates()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        _apiMock.Setup(x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException(cts.Token));

        var sut = CreateSut();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow, cts.Token));
    }

    // Проверяет, что при ошибке API провайдер не кэширует неудачный результат:
    // после исключения следующий вызов снова идёт в API.

    [Fact]
    public async Task GetCurrentWeatherAsync_WhenApiThrows_DoesNotCacheError()
    {
        // Arrange
        _apiMock.SetupSequence(x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromException<WeatherData>(new HttpRequestException("boom")))
                .Returns(() => Task.FromResult(CreateDefaultWeatherData()));

        var sut = CreateSut();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow));

        var result = await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);

        result.Should().NotBeNull();
        _apiMock.Verify(
            x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    // Проверяет, что CancellationToken, переданный в провайдер,
    // доходит до вызова API без изменений.

    [Fact]
    public async Task GetCurrentWeatherAsync_PassesCancellationToken_ToApi()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        CancellationToken captured = default;

        _apiMock.Setup(x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()))
                .Callback<double, double, CancellationToken>((_, _, ct) => captured = ct)
                .Returns(() => Task.FromResult(CreateDefaultWeatherData()));

        var sut = CreateSut();

        // Act
        await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow, cts.Token);

        // Assert
        captured.Should().Be(cts.Token);
    }

    // Проверяет, что для прогноза используется ForecastCacheTtl,
    // а не WeatherCacheTtl: после истечения weather TTL
    // прогноз остаётся в кэше.

    [Fact]
    public async Task GetForecastAsync_UsesForecastTtl_NotWeatherTtl()
    {
        // Arrange
        CreateAndSetupDefaultCurrentWeather();
        CreateAndSetupForecast(3);

        var sut = CreateSut(
            weatherTtl: TimeSpan.FromMilliseconds(10),
            forecastTtl: TimeSpan.FromMinutes(10));

        await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);
        await sut.GetForecastAsync(LatMoscow, LonMoscow, 3);

        // Act — двигаем время так, чтобы weather истёк, а forecast — нет
        _timeProvider.Advance(TimeSpan.FromMilliseconds(50));
        await sut.GetForecastAsync(LatMoscow, LonMoscow, 3);

        // Assert — forecast должен остаться в кэше
        _apiMock.Verify(
            x => x.GetForecastAsync(LatMoscow, LonMoscow, 3, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // Проверяет, что для текущей погоды используется WeatherCacheTtl,
    // а не ForecastCacheTtl: в пределах weather TTL
    // повторный вызов берётся из кэша.

    [Fact]
    public async Task GetCurrentWeatherAsync_UsesWeatherTtl_NotForecastTtl()
    {
        // Arrange
        CreateAndSetupDefaultCurrentWeather();

        var sut = CreateSut(
            weatherTtl: TimeSpan.FromMinutes(10),
            forecastTtl: TimeSpan.FromMilliseconds(10));

        await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);

        // Act — двигаем время в пределах weather TTL
        _timeProvider.Advance(TimeSpan.FromMilliseconds(50));
        await sut.GetCurrentWeatherAsync(LatMoscow, LonMoscow);

        // Assert — weather должен остаться в кэше
        _apiMock.Verify(
            x => x.GetCurrentWeatherAsync(LatMoscow, LonMoscow, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ============ Фабрики ============

    private static WeatherData CreateWeatherData(string city, double lat, double lon)
    {
        var location = new Location(city, lat, lon);
        var current = new CurrentWeather(
            DateTime.UtcNow,
            Domain.ValueObjects.Temperature.FromCelsius(22.5),
            1000, "https://example.com/sunny.png", "Sunny",
            Domain.ValueObjects.WindSpeed.FromKph(10.5),
            65,
            Domain.ValueObjects.Temperature.FromCelsius(20.0),
            5.0);
        return new WeatherData(location, current);
    }

    private static WeatherData CreateDefaultWeatherData()
    {
        return CreateWeatherData("Moscow", LatMoscow, LonMoscow);
    }

    private static ForecastData CreateForecastData()
    {
        var location = new Location("Moscow", LatMoscow, LonMoscow);
        return new ForecastData(location,
            new List<HourlyForecast>(),
            new List<DailyForecast>());
    }

    private void SetupCurrentWeather(WeatherData data)
    {
        _apiMock
            .Setup(x => x.GetCurrentWeatherAsync(data.Location.Latitude, data.Location.Longitude, It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(data));
    }

    private void SetupForecast(ForecastData data, int dayCount = 3)
    {
        _apiMock
            .Setup(x => x.GetForecastAsync(data.Location.Latitude, data.Location.Longitude, dayCount, It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(data));
    }

    private void CreateAndSetupDefaultCurrentWeather()
    {
        WeatherData data = CreateWeatherData("Moscow", LatMoscow, LonMoscow);
        SetupCurrentWeather(data);
    }

    private void CreateAndSetupForecast(int dayCount)
    {
        ForecastData data = CreateForecastData();
        SetupForecast(data, dayCount);
    }
}