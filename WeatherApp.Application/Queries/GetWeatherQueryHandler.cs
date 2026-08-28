using ErrorOr;
using MediatR;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Domain.Entities;


namespace WeatherApp.Application.Queries
{
    public class GetWeatherQueryHandler : IRequestHandler<GetWeatherQuery, ErrorOr<WeatherData>>
    {
        private readonly IWeatherApiClient _weatherApiClient;

        public GetWeatherQueryHandler(IWeatherApiClient weatherApiClient)
        {
            _weatherApiClient = weatherApiClient;
        }

        public async Task<ErrorOr<WeatherData>> Handle(GetWeatherQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Просто вызываем наш интерфейс и возвращаем результат
                var weatherData = await _weatherApiClient.GetWeatherDataAsync(request.Latitude, request.Longitude, cancellationToken);
                return weatherData;
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
