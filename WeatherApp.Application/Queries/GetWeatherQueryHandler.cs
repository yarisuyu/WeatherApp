using ErrorOr;
using Mapster;
using MediatR;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.DTOs;


namespace WeatherApp.Application.Queries
{
    public class GetWeatherQueryHandler : IRequestHandler<GetCurrentWeatherQuery, ErrorOr<CurrentWeatherResponseDto>>
    {
        private readonly IWeatherDataProvider _provider;

        public GetWeatherQueryHandler(IWeatherDataProvider provider)
        {
            _provider = provider;
        }

        public async Task<ErrorOr<CurrentWeatherResponseDto>> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var weatherData = await _provider.GetCurrentWeatherAsync(request.Latitude, request.Longitude, cancellationToken);
                var responseDto = weatherData.Adapt<CurrentWeatherResponseDto>();

                return responseDto;
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
