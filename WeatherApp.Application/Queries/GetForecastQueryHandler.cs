using ErrorOr;
using MapsterMapper;
using MediatR;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.DTOs;


namespace WeatherApp.Application.Queries;

public class GetForecastQueryHandler : IRequestHandler<GetForecastQuery, ErrorOr<ForecastResponseDto>>
{
    private readonly IWeatherDataProvider _provider;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public GetForecastQueryHandler(
        IWeatherDataProvider provider,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _provider = provider;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<ErrorOr<ForecastResponseDto>> Handle(GetForecastQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var forecastData = await _provider.GetForecastAsync(request.Latitude, request.Longitude, request.DayCount, cancellationToken);
            
            if (forecastData is null)
            {
                return Error.Unexpected(
                    code: "Weather.ProviderReturnedNull",
                    description: "Провайдер вернул пустой результат.");
            }

            // Фильтруем часы: оставшиеся сегодня + все завтра
            var now = _timeProvider.GetLocalNow().DateTime;
            var filteredHourly = forecastData
                .GetRemainingHoursTodayAndTomorrow(now)
                .ToList();

            // Маппим только нужные части
            var dto = new ForecastResponseDto
            {
                Location = _mapper.Map<LocationDto>(forecastData.Location),
                HourlyForecasts = _mapper.Map<List<HourlyForecastDto>>(filteredHourly),
                DailyForecasts = _mapper.Map<List<DailyForecastDto>>(forecastData.DailyForecasts)
            };

            return dto;
        }
        catch (HttpRequestException ex)
        {
            // Возвращаем ошибку, а не выбрасываем исключение
            return Error.Failure("WeatherApi.Failed", $"Не удалось получить данные о погоде: {ex.Message}");
        }
        catch (Exception)
        {
            return Error.Unexpected("Internal.ServerError", "Произошла внутренняя ошибка.");
        }
    }
}
