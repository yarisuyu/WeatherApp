using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Domain.Entities
{
    public sealed record Location
    {
        public string City { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        public Location(string city, double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("Название города не должно быть пустым.", nameof(city));
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Значение широты должно быть между -90 и 90.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Значение долготы должно быть между -180 и 180.");

            City = city;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
