using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.Entities
{
    public sealed class HourlyForecast
    {
        public DateTime Time { get; }
        public Temperature Temperature { get; }
        public int ConditionCode { get; }
        public string ConditionText { get; }
        public WindSpeed WindSpeed { get; }
        public int HumidityPercent { get; }

        public HourlyForecast(
            DateTime time,
            Temperature temperature,
            int conditionCode,
            string conditionText,
            WindSpeed windSpeed,
            int humidityPercent)
        {
            // Валидация
            if (humidityPercent < 0 || humidityPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(humidityPercent), "Humidity value must be between 0 and 100%.");

            Time = time;
            Temperature = temperature;
            ConditionCode = conditionCode;
            ConditionText = conditionText;
            WindSpeed = windSpeed;
            HumidityPercent = humidityPercent;
        }
    }
}
