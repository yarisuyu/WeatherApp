using FluentValidation;
using WeatherApp.Application.Queries;

public class GetForecastQueryValidator : AbstractValidator<GetForecastQuery>
{
    public GetForecastQueryValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.DayCount).InclusiveBetween(1, 14);
    }
}