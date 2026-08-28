using FluentValidation;
using WeatherApp.Application.Queries;

public class GetWeatherQueryValidator : AbstractValidator<GetWeatherQuery>
{
    public GetWeatherQueryValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}