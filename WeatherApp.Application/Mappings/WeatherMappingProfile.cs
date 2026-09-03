using Mapster;
using WeatherApp.Application.DTOs;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Mappings
{
    public class WeatherMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Маппинг Location -> LocationDto
            config.NewConfig<Location, LocationDto>()
                .Map(dest => dest.City, src => src.City);

            // Маппинг CurrentWeather -> CurrentWeatherDto
            config.NewConfig<CurrentWeather, CurrentWeatherDto>()
                .Map(dest => dest.TemperatureCelsius, src => src.Temperature.Celsius)
                .Map(dest => dest.FeelsLikeCelsius, src => src.FeelsLike.Celsius)
                .Map(dest => dest.WindSpeedKph, src => src.WindSpeed.Kph);

            // Маппинг HourlyForecast -> HourlyForecastDto
            config.NewConfig<HourlyForecast, HourlyForecastDto>()
                .Map(dest => dest.TemperatureCelsius, src => src.Temperature.Celsius)
                .Map(dest => dest.WindSpeedKph, src => src.WindSpeed.Kph);

            // Маппинг DailyForecast -> DailyForecastDto
            config.NewConfig<DailyForecast, DailyForecastDto>()
                .Map(dest => dest.MaxTemperatureCelsius, src => src.MaxTemperature.Celsius)
                .Map(dest => dest.MinTemperatureCelsius, src => src.MinTemperature.Celsius)
                .Map(dest => dest.AvgTemperatureCelsius, src => src.AvgTemperature.Celsius);

            // Маппинг WeatherData -> WeatherRespoonseDto
            config.NewConfig<WeatherData, CurrentWeatherResponseDto>()
                .Map(dest => dest.Location, src => src.Location)
                .Map(dest => dest.CurrentWeather, src => src.Current);

            // Маппинг ForecastData -> ForecastRespoonseDto
            config.NewConfig<ForecastData, ForecastResponseDto>()
                .Map(dest => dest.Location, src => src.Location)
                .Map(dest => dest.HourlyForecasts, src => src.HourlyForecasts)
                .Map(dest => dest.DailyForecasts, src => src.DailyForecasts);
        }
    }
}