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
                throw new ArgumentException("City name cannot be empty.", nameof(city));
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "latitude value must be between -90 and 90.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude value must be between -180 and 180.");

            City = city;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
