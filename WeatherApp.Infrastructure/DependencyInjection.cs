using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Infrastructure.Options;
using WeatherApp.Infrastructure.Services;

namespace WeatherApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Регистрируем опции
            services.Configure<WeatherApiOptions>(configuration.GetSection("WeatherApi"));

            // Регистрируем HttpClient и клиент
            services.AddHttpClient<IWeatherApiClient, WeatherApiClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<WeatherApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}
