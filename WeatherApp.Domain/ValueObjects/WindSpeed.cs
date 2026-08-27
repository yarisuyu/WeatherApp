namespace WeatherApp.Domain.ValueObjects
{
    public readonly record struct WindSpeed
    {
        public double Kph { get; }
        public double Mps => Kph / 3.6;

        private WindSpeed(double kph)
        {
            if (kph < 0)
                throw new ArgumentException("Скорость ветра не может быть отрицательной.", nameof(kph));
            Kph = kph;
        }

        public static WindSpeed FromKph(double kph) => new(kph);
        public static WindSpeed FromMps(double mps) => new(mps * 3.6);
    }
}
