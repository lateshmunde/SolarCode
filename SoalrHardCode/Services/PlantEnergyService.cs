using System;
using System.Collections.Generic;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class PlantEnergyService
    {
        private readonly ISunPositionService _sunService;
        private readonly IShadingService _shadingService;
        private readonly IEnergyCalculationService _energyService;

        public PlantEnergyService(
            ISunPositionService sunService,
            IShadingService shadingService,
            IEnergyCalculationService energyService)
        {
            _sunService = sunService;
            _shadingService = shadingService;
            _energyService = energyService;
        }

        /// <summary>
        /// Calculates hourly energy for the entire plant.
        /// This is the lowest-level aggregation.
        /// </summary>
        private double CalculateHourlyPlantEnergy(
            Plant plant,
            SolarIrradiance irradiance)
        {
            double hourEnergy = 0;

            double sunAltitude =
                _sunService.GetSolarAltitudeDeg(irradiance.Hour);

            foreach (var panel in plant.Panels)
            {
                double shadingLoss =
                    _shadingService.GetShadingLoss(
                        panel.HeightMeters,
                        sunAltitude
                    );

                hourEnergy += _energyService.CalculateHourlyEnergy(
                    panel,
                    irradiance,
                    sunAltitude,
                    shadingLoss
                );
            }

            return hourEnergy;
        }

        /// <summary>
        /// Aggregates hourly data into monthly energy totals.
        /// </summary>
        public IReadOnlyList<MonthlyEnergyResult> CalculateMonthlyEnergy(
            Plant plant,
            IEnumerable<SolarIrradiance> hourlyData,
            int year)
        {
            var monthlyTotals = new Dictionary<int, double>();

            foreach (var data in hourlyData)
            {
                // NASA timestamp is not stored; assume representative year
                int month = EstimateMonthFromHour(data.Hour, year);

                if (!monthlyTotals.ContainsKey(month))
                    monthlyTotals[month] = 0;

                monthlyTotals[month] +=
                    CalculateHourlyPlantEnergy(plant, data);
            }

            var results = new List<MonthlyEnergyResult>();

            foreach (var kvp in monthlyTotals)
            {
                results.Add(
                    new MonthlyEnergyResult(
                        kvp.Key,
                        kvp.Value
                    )
                );
            }

            results.Sort((a, b) => a.Month.CompareTo(b.Month));
            return results;
        }

        /// <summary>
        /// Temporary helper for POC.
        /// 
        /// In real implementation, SolarIrradiance will
        /// carry DateTime directly.
        /// </summary>
        private int EstimateMonthFromHour(int hour, int year)
        {
            // Simple assumption: equal distribution
            // Will be replaced once timestamp is DateTime
            return (hour % 12) + 1;
        }

        /// <summary>
        /// Aggregates monthly results into yearly energy.
        /// </summary>
        public double CalculateYearlyEnergy(
            IEnumerable<MonthlyEnergyResult> monthlyResults)
        {
            double total = 0;
            foreach (var month in monthlyResults)
                total += month.EnergyKWh;

            return total;
        }
    }
}
