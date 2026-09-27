using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Application.Services;

namespace WeatherApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Регистрация Mapster
            var mapsterConfig = TypeAdapterConfig.GlobalSettings;
            mapsterConfig.Scan(assembly);

            services.AddSingleton(mapsterConfig);
            services.AddScoped<IMapper, ServiceMapper>();

            // Регистрация MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                // Регистрация Pipeline Behaviors в правильном порядке
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddMemoryCache();
            services.Configure<WeatherDataProviderOptions>(
                configuration.GetSection(WeatherDataProviderOptions.SectionName));
            services.AddSingleton(TimeProvider.System);
            services.AddScoped<IWeatherDataProvider, WeatherDataProvider>();

            // Регистрация FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
