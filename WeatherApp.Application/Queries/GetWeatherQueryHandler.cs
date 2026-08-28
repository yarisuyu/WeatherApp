using ErrorOr;
using MediatR;
using Mapster;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.DTOs;


namespace WeatherApp.Application.Queries
{
    public class GetWeatherQueryHandler : IRequestHandler<GetWeatherQuery, ErrorOr<WeatherDashboardDto>>
    {
        private readonly IWeatherApiClient _weatherApiClient;

        public GetWeatherQueryHandler(IWeatherApiClient weatherApiClient)
        {
            _weatherApiClient = weatherApiClient;
        }

        public async Task<ErrorOr<WeatherDashboardDto>> Handle(GetWeatherQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Просто вызываем наш интерфейс и возвращаем результат
                var weatherData = await _weatherApiClient.GetWeatherDataAsync(request.Latitude, request.Longitude, cancellationToken);
                // 2. Маппим доменную сущность в DTO
                var dashboardDto = weatherData.Adapt<WeatherDashboardDto>();
                Console.Write(weatherData);

                // 3. Возвращаем успешный результат
                return dashboardDto;
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
