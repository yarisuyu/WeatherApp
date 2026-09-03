using ErrorOr;
using MediatR;
using Mapster;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.DTOs;


namespace WeatherApp.Application.Queries
{
    public class GetForecastHandler : IRequestHandler<GetForecastQuery, ErrorOr<ForecastResponseDto>>
    {
        private readonly IWeatherApiClient _weatherApiClient;

        public GetForecastHandler(IWeatherApiClient weatherApiClient)
        {
            _weatherApiClient = weatherApiClient;
        }

        public async Task<ErrorOr<ForecastResponseDto>> Handle(GetForecastQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Просто вызываем наш интерфейс и возвращаем результат
                var forecastData = await _weatherApiClient.GetForecastAsync(request.Latitude, request.Longitude, request.DayCount, cancellationToken);
                // 2. Маппим доменную сущность в DTO
                var forecastDto = forecastData.Adapt<ForecastResponseDto>();

                // 3. Возвращаем успешный результат
                return forecastDto;
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
}
