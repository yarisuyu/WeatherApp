using ErrorOr;
using Mapster;
using MediatR;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.DTOs;


namespace WeatherApp.Application.Queries
{
    public class GetForecastHandler : IRequestHandler<GetForecastQuery, ErrorOr<ForecastResponseDto>>
    {
        private readonly IWeatherDataProvider _provider;

        public GetForecastHandler(IWeatherDataProvider provider)
        {
            _provider = provider;
        }

        public async Task<ErrorOr<ForecastResponseDto>> Handle(GetForecastQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var forecastData = await _provider.GetForecastAsync(request.Latitude, request.Longitude, request.DayCount, cancellationToken);
                var forecastDto = forecastData.Adapt<ForecastResponseDto>();

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
