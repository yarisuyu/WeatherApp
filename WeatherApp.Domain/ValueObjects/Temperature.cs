namespace WeatherApp.Domain.ValueObjects
{
    public readonly struct Temperature
    {
        public double Celsius { get; }

        private Temperature(double celsius)
        {
            if (celsius < -273.15)
                throw new ArgumentException("Temperature cannot be below absolute zero.", nameof(celsius));
            Celsius = celsius;
        }

        public static Temperature FromCelsius(double celsius) => new(celsius);
    }
}
