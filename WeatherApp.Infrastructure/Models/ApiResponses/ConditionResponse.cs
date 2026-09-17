using System.Text.Json.Serialization;

namespace WeatherApp.Infrastructure.Models.ApiResponses
{
    public class ConditionResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
