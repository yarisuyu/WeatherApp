using System.Globalization;
using WeatherApp.Web.Formatters;
using Xunit;

namespace WeatherApp.Web.UnitTests.Formatters
{
    public class TemperatureFormatterTests
    {
        [Fact]
        public void ToTemperatureDisplay_Positive_ShouldShowPlus()
        {
            var value = 5.2;
            var result = TemperatureFormatter.ToTemperatureDisplay(value);
            var expected = $"+{Math.Round(value, 1).ToString(CultureInfo.CurrentCulture)}°C";
            Assert.Contains(expected, result);
        }

        [Fact]
        public void ToTemperatureDisplay_Negative_ShouldShowMinus()
        {
            var value = -3.4;
            var result = TemperatureFormatter.ToTemperatureDisplay(value);
            var expected = $"{Math.Round(value, 1).ToString(CultureInfo.CurrentCulture)}°C";
            Assert.Contains(expected, result);
        }
    }
}
