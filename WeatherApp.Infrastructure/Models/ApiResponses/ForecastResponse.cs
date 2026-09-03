using System.Text.Json.Serialization;
using WeatherApp.Infrastructure.Models.ApiResponses;

namespace WeatherApp.Infrastructure.Models
{
    public class ForecastResponse
    {
        [JsonPropertyName("location")]
        public LocationResponse Location { get; set; } = null!;
        [JsonPropertyName("forecast")]
        public ForecastContainer Forecast { get; set; } = null!;
    }
    public class LocationResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }

    public class ForecastContainer
    {
        [JsonPropertyName("forecastday")]
        public List<ForecastDay> ForecastDay { get; set; } = new();
    }

    public class ForecastDay
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("day")]
        public DayData Day { get; set; } = null!;

        [JsonPropertyName("hour")]
        public List<HourData> Hour { get; set; } = new();
    }

    public class DayData
    {
        [JsonPropertyName("maxtemp_c")]
        public double MaxTempC { get; set; }

        [JsonPropertyName("mintemp_c")]
        public double MinTempC { get; set; }

        [JsonPropertyName("avgtemp_c")]
        public double AvgTempC { get; set; }

        [JsonPropertyName("condition")]
        public ConditionResponse Condition { get; set; } = null!;

        [JsonPropertyName("totalprecip_mm")]
        public double TotalPrecipMm { get; set; }
    }

    public class HourData
    {
        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;

        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("condition")]
        public ConditionResponse Condition { get; set; } = null!;

        [JsonPropertyName("wind_kph")]
        public double WindKph { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

}