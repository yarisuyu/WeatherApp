namespace WeatherApp.Domain.ValueObjects
{
    public readonly struct Temperature
    {
        public double Celsius { get; }

        private Temperature(double celsius)
        {
            if (celsius < -273.15)
                throw new ArgumentException("Температура по Цельсию не должна быть ниже абсолютного нуля.", nameof(celsius));
            Celsius = celsius;
        }

        public static Temperature FromCelsius(double celsius) => new(celsius);

        public override string ToString() => $"{Celsius:F1}°C";
    }
}
