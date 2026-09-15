using FluentAssertions;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.UnitTests.Entities;

public class ForecastDataTests
{
    private readonly Location _location = new("Moscow", 55.7558, 37.6173);
    private readonly List<HourlyForecast> _hourly;
    private readonly List<DailyForecast> _daily;

    private static ForecastData CreateForecastData(params DateTime[] hourTimes)
    {
        var location = new Location("Moscow", 55.7558, 37.6173);
        var hourly = hourTimes
            .Select(t => new HourlyForecast(
                t,
                Temperature.FromCelsius(15),
                1000, "Sunny",
                WindSpeed.FromKph(5),
                60))
            .ToList();
        return new ForecastData(location, hourly, new List<DailyForecast>());
    }

    public ForecastDataTests()
    {
        _hourly = new List<HourlyForecast>
        {
            new(new DateTime(2026, 9, 10, 10, 0, 0), Temperature.FromCelsius(18), 1000, "Sunny", WindSpeed.FromKph(5), 55),
            new(new DateTime(2026, 9, 10, 12, 0, 0), Temperature.FromCelsius(20), 1000, "Sunny", WindSpeed.FromKph(6), 50),
            new(new DateTime(2026, 9, 10, 14, 0, 0), Temperature.FromCelsius(22), 1000, "Sunny", WindSpeed.FromKph(7), 45),
        };

        _daily = new List<DailyForecast>
        {
            new(new DateTime(2026, 9, 10), Temperature.FromCelsius(25), Temperature.FromCelsius(15), Temperature.FromCelsius(20), 1000, "Sunny", 0),
            new(new DateTime(2026, 9, 11), Temperature.FromCelsius(23), Temperature.FromCelsius(14), Temperature.FromCelsius(18), 1003, "Cloudy", 1.5),
            new(new DateTime(2026, 9, 12), Temperature.FromCelsius(20), Temperature.FromCelsius(12), Temperature.FromCelsius(16), 1063, "Rain", 5.0),
        };
    }

    [Fact]
    public void Constructor_WithValidData_CreatesInstance()
    {
        var data = new ForecastData(_location, _hourly, _daily);

        data.Location.Should().Be(_location);
        data.HourlyForecasts.Should().HaveCount(3);
        data.DailyForecasts.Should().HaveCount(3);
    }

    [Fact]
    public void Constructor_WhenLocationNull_ThrowsArgumentNullException()
    {
        Action act = () => new ForecastData(null!, _hourly, _daily);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("location");
    }

    [Fact]
    public void Constructor_WhenHourlyNull_ThrowsArgumentNullException()
    {
        Action act = () => new ForecastData(_location, null!, _daily);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("hourlyForecasts");
    }

    [Fact]
    public void Constructor_WhenDailyNull_ThrowsArgumentNullException()
    {
        Action act = () => new ForecastData(_location, _hourly, null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dailyForecasts");
    }

    [Fact]
    public void GetRemainingHoursTodayAndTomorrow_IncludesRemainingTodayAndAllTomorrow()
    {
        var now = new DateTime(2026, 9, 10, 12, 0, 0);
        var data = CreateForecastData(
            now.AddHours(-3),                     // прошлое сегодня — отсечь
            now,                                  // сейчас — оставить
            now.AddHours(2),                      // позже сегодня — оставить
            now.Date.AddDays(1).AddHours(0),      // завтра 00:00 — оставить
            now.Date.AddDays(1).AddHours(12),     // завтра 12:00 — оставить
            now.Date.AddDays(2).AddHours(0));     // послезавтра — отсечь

        var result = data.GetRemainingHoursTodayAndTomorrow(now).ToList();

        result.Should().HaveCount(4);
        result.Select(h => h.Time).Should().BeInAscendingOrder();
    }

    [Fact]
    public void GetRemainingHoursTodayAndTomorrow_ExcludesPastHoursOfToday()
    {
        var now = new DateTime(2026, 9, 10, 15, 0, 0);
        var data = CreateForecastData(
            now.Date.AddHours(8),
            now.Date.AddHours(10),
            now.Date.AddHours(14));   // всё в прошлом

        var result = data.GetRemainingHoursTodayAndTomorrow(now).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetRemainingHoursTodayAndTomorrow_IncludesCurrentHour()
    {
        var now = new DateTime(2026, 9, 10, 15, 0, 0);
        var data = CreateForecastData(now);  // час ровно "сейчас"

        var result = data.GetRemainingHoursTodayAndTomorrow(now).ToList();

        result.Should().ContainSingle()
            .Which.Time.Should().Be(now);
    }

    [Fact]
    public void GetRemainingHoursTodayAndTomorrow_ExcludesHoursAfterTomorrow()
    {
        var now = new DateTime(2026, 9, 10, 12, 0, 0);
        var data = CreateForecastData(
            now.Date.AddDays(2).AddHours(10),
            now.Date.AddDays(3).AddHours(10));

        var result = data.GetRemainingHoursTodayAndTomorrow(now).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetRemainingHoursTodayAndTomorrow_ReturnsTomorrowHours()
    {
        var now = new DateTime(2026, 9, 10, 23, 0, 0);
        var tomorrow = now.Date.AddDays(1);
        var data = CreateForecastData(
            tomorrow.AddHours(0),
            tomorrow.AddHours(6),
            tomorrow.AddHours(12),
            tomorrow.AddHours(23));

        var result = data.GetRemainingHoursTodayAndTomorrow(now).ToList();

        result.Should().HaveCount(4);
        result.All(h => h.Time.Date == tomorrow).Should().BeTrue();
    }

    [Fact]
    public void GetRemainingHoursTodayAndTomorrow_WithEmptyList_ReturnsEmpty()
    {
        var data = CreateForecastData();  // пустой список

        var result = data.GetRemainingHoursTodayAndTomorrow(new DateTime(2026, 9, 10));

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetRemainingHoursTodayAndTomorrow_OrdersByTime()
    {
        var now = new DateTime(2026, 9, 10, 12, 0, 0);
        var data = CreateForecastData(
            now.Date.AddDays(1).AddHours(6),
            now.AddHours(2),
            now,
            now.Date.AddDays(1).AddHours(0),
            now.AddHours(5));

        var result = data.GetRemainingHoursTodayAndTomorrow(now).ToList();

        result.Select(h => h.Time).Should().BeInAscendingOrder();
    }

    [Fact]
    public void GetRemainingHoursTodayAndTomorrow_ExcludesYesterday()
    {
        var now = new DateTime(2026, 9, 10, 12, 0, 0);
        var data = CreateForecastData(
            now.Date.AddDays(-1).AddHours(10),
            now.Date.AddDays(-1).AddHours(20));

        var result = data.GetRemainingHoursTodayAndTomorrow(now).ToList();

        result.Should().BeEmpty();
    }
}