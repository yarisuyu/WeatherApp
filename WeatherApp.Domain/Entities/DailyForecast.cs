using System;
using System.Collections.Generic;
using System.Text;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Domain.Entities
{
    public sealed class DailyForecast
    {
        public DateTime Date { get; }
        public Temperature MaxTemperature { get; }
        public Temperature MinTemperature { get; }
        public Temperature AvgTemperature { get; }
        public int ConditionCode { get; }
        public string ConditionText { get; }
        public double TotalPrecipitationMm { get; }

        public DailyForecast(
            DateTime date,
            Temperature maxTemperature,
            Temperature minTemperature,
            Temperature avgTemperature,
            int conditionCode,
            string conditionText,
            double totalPrecipitationMm)
        {
            if (totalPrecipitationMm < 0)
                throw new ArgumentOutOfRangeException(nameof(totalPrecipitationMm), "Количество осадков не может быть отрицательным.");

            Date = date;
            MaxTemperature = maxTemperature;
            MinTemperature = minTemperature;
            AvgTemperature = avgTemperature;
            ConditionCode = conditionCode;
            ConditionText = conditionText;
            TotalPrecipitationMm = totalPrecipitationMm;
        }
    }
}
