using System;
using System.Collections.Generic;
using System.Text;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.Entities
{
    public sealed class CurrentWeather
    {
        public DateTime LastUpdated { get; }
        public Temperature Temperature { get; }
        public int ConditionCode { get; }
        public string ConditionText { get; }
        public WindSpeed WindSpeed { get; }
        public int HumidityPercent { get; }
        public Temperature FeelsLike { get; }
        public double UvIndex { get; }

        public CurrentWeather(
            DateTime lastUpdated,
            Temperature temperature,
            int conditionCode,
            string conditionText,
            WindSpeed windSpeed,
            int humidityPercent,
            Temperature feelsLike,
            double uvIndex)
        {
            LastUpdated = lastUpdated;
            Temperature = temperature;
            ConditionCode = conditionCode;
            ConditionText = conditionText;
            WindSpeed = windSpeed;
            HumidityPercent = humidityPercent;
            FeelsLike = feelsLike;
            UvIndex = uvIndex;

            // Валидация дополнительных инвариантов
            if (humidityPercent < 0 || humidityPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(humidityPercent), "Влажность должна быть в интервале 0-100%.");
            if (uvIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(uvIndex), "UV индекс не может быть отрицательным.");
        }
    }
}
