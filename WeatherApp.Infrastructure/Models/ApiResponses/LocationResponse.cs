using System.Text.Json.Serialization;

namespace WeatherApp.Infrastructure.Models.ApiResponses
{
    public class LocationResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }
}
