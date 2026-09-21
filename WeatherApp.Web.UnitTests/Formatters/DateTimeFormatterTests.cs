using WeatherApp.Web.Formatters;
using Xunit;

namespace WeatherApp.Web.UnitTests.Formatters
{
    public class DateTimeFormatterTests
    {
        [Fact]
        public void ToDayDisplay_Today_ReturnsToday()
        {
            var res = DateTimeFormatter.ToDayDisplay(DateTime.Today);
            Assert.Contains("Сегодня", res);
        }
    }
}
