using ErrorOr;
using MapsterMapper;
using MediatR;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.DTOs;


namespace WeatherApp.Application.Queries;

public class GetWeatherQueryHandler : IRequestHandler<GetCurrentWeatherQuery, ErrorOr<CurrentWeatherResponseDto>>
{
    private readonly IWeatherDataProvider _provider;
    private readonly IMapper _mapper;

    public GetWeatherQueryHandler(IWeatherDataProvider provider, IMapper mapper)
    {
        _provider = provider;
        _mapper = mapper;
    }

    public async Task<ErrorOr<CurrentWeatherResponseDto>> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var weatherData = await _provider.GetCurrentWeatherAsync(request.Latitude, request.Longitude, cancellationToken);

            if (weatherData is null)
            {
                return Error.Unexpected(
                    code: "Weather.ProviderReturnedNull",
                    description: "Провайдер вернул пустой результат.");
            }

            var responseDto = _mapper.Map<CurrentWeatherResponseDto>(weatherData);

            if (responseDto is null)
            {
                return Error.Unexpected(
                    code: "Weather.MappingFailed",
                    description: "Не удалось преобразовать данные о погоде.");
            }

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
