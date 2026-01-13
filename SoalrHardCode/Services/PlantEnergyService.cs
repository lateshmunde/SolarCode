using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;
using System.Collections.Generic;

namespace SolarEnergyPOC.Services
{
    public class PlantEnergyService
    {
        private readonly SunPositionService Sun;
        private readonly IShadingService Shading;
        private readonly IEnergyCalculationService Energy;

        public PlantEnergyService(
            SunPositionService sun,
            IShadingService shading,
            IEnergyCalculationService energy)
        {
            Sun = sun;
            Shading = shading;
            Energy = energy;
        }

        public IReadOnlyList<MonthlyEnergyResult> CalculateMonthlyEnergy(
            Plant plant,
            IEnumerable<SolarIrradiance> data)
        {
            var map = new Dictionary<int, double>();

            foreach (var d in data)
            {
                int month = d.DateTimeLocal.Month;
                if (!map.ContainsKey(month))
                    map[month] = 0.0;

                double sunAlt = Sun.GetSolarAltitude(d.DateTimeLocal);

                foreach (var panel in plant.Panels)
                {
                    double shadingLoss =
                        Shading.GetShadingLoss(panel.HeightMeters, sunAlt);

                    map[month] +=
                        Energy.CalculateHourlyEnergy(panel, d, sunAlt, shadingLoss);
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
