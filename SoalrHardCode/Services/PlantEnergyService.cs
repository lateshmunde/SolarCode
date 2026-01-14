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

        public double CalculateAnnualEnergy(Plant plant, IReadOnlyList<MonthlyEnergyResult> data)
        {
            var monthly = data;

            double total = 0;
            foreach (var m in monthly)
                total += m.EnergyKWh;

            return total;
        }

    }
}