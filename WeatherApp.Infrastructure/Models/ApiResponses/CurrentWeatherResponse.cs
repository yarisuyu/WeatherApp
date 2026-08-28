using System.Text.Json.Serialization;

namespace WeatherApp.Infrastructure.Models.ApiResponses
{
    public class CurrentWeatherResponse
    {
        [JsonPropertyName("location")]
        public LocationResponse Location { get; set; } = null!;

        [JsonPropertyName("current")]
        public CurrentResponse Current { get; set; } = null!;
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

    public class CurrentResponse
    {
        [JsonPropertyName("last_updated")]
        public string LastUpdated { get; set; } = string.Empty;

        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("condition")]
        public ConditionResponse Condition { get; set; } = null!;

        [JsonPropertyName("wind_kph")]
        public double WindKph { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }

        [JsonPropertyName("feelslike_c")]
        public double FeelsLikeC { get; set; }

        [JsonPropertyName("uv")]
        public double Uv { get; set; }
    }

    public class ConditionResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
