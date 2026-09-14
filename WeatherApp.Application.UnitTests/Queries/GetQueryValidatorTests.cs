using FluentValidation.TestHelper;
using WeatherApp.Application.Queries;

namespace WeatherApp.Application.UnitTests.Queriess;

public class GetWeatherQueryValidatorTests
{
    private readonly GetWeatherQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidLatLon_ShouldNotHaveErrors()
    {
        var query = new GetCurrentWeatherQuery(55.7558, 37.6173);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithLatOutOfRange_ShouldHaveErrorForLatitude()
    {
        var query = new GetCurrentWeatherQuery(100, 37);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorMessage("'Latitude' должно быть в диапазоне от -90 до 90. Введенное значение: 100.");
    }

    [Fact]
    public void Validate_WithLonOutOfRange_ShouldHaveErrorForLongitude()
    {
        var query = new GetCurrentWeatherQuery(55, 200);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Longitude)
            .WithErrorMessage("'Longitude' должно быть в диапазоне от -180 до 180. Введенное значение: 200.");
    }
}