using System;
using System.Collections.Generic;
using System.Linq;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace SolarEnergyPOC.Services
{
    public class PlantEnergyService
    {
        private readonly SunPositionService Sun;
        private readonly IShadingService Shading; // double GetShadingLoss
        private readonly IEnergyCalculationService Energy; // double CalculateHourlyEnergy

        public PlantEnergyService(SunPositionService sun, IShadingService shading, IEnergyCalculationService energy)
        {
            Sun = sun;
            Shading = shading;
            Energy = energy;
        }
        //IEnumerable - provides a standard way to iterate over a sequence of data, allowing developers to treat different collection 
        // types(like arrays, lists, or custom collections) in a uniform manner using loops
        public IReadOnlyList<MonthlyEnergyResult> CalculateMonthlyEnergy(Plant plant, IEnumerable<SolarIrradiance> data)
        {
            var map = new Dictionary<int, double>();

            foreach (var d in data)
            {
                int month = d.DateTimeLocal.Month;
                if (!map.ContainsKey(month)) map[month] = 0;

                double alt = Sun.GetSolarAltitude(d.DateTimeLocal); //angle above horizon

                foreach (var p in plant.Panels)
                {
                    double shade = Shading.GetShadingLoss(p.HeightMeters, alt);

                    map[month] += Energy.CalculateHourlyEnergy(p, d, alt, shade);
                }

                monthlyEnergy *= panelCount;

                int daysInMonth =
                    DateTime.DaysInMonth(
                        monthGroup.First().Timestamp.Year,
                        monthGroup.Key);

                double plantCapacityKW = panelCount * 0.54;

                double specificYield =
                    monthlyEnergy / plantCapacityKW;

                double cuf =
                    monthlyEnergy /
                    (plantCapacityKW * 24 * daysInMonth);

                results.Add(
                    new MonthlyEnergyResult(
                        monthGroup.Key,
                        monthlyEnergy,
                        specificYield,
                        cuf));
            }

            var result = new List<MonthlyEnergyResult>();
            foreach (var kv in map)
                result.Add(new MonthlyEnergyResult(kv.Key, kv.Value));

            result.Sort((a, b) => a.Month.CompareTo(b.Month));
            return result;
        }

        public IReadOnlyList<MonthlyEnergyResult> CalculateMonthlyEnergyIdeal(Plant plant, IEnumerable<SolarIrradiance> data)
        {
            var map = new Dictionary<int, double>();

            foreach (var d in data)
            {
                int month = d.DateTimeLocal.Month;
                if (!map.ContainsKey(month)) map[month] = 0;

                double alt = Sun.GetSolarAltitude(d.DateTimeLocal); //angle above horizon

                foreach (var p in plant.Panels)
                {
                    double shade = Shading.GetShadingLossIdeal(p.HeightMeters, alt);

                    map[month] += Energy.CalculateHourlyEnergyIdeal(p, d, alt, shade);
                }
            }

            var result = new List<MonthlyEnergyResult>();
            foreach (var kv in map)
                result.Add(new MonthlyEnergyResult(kv.Key, kv.Value));

            result.Sort((a, b) => a.Month.CompareTo(b.Month));
            return result;
        }
    }
}