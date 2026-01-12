using System.Collections.Generic;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class PlantEnergyService
    {
        private readonly SunPositionService _sun;
        private readonly IShadingService _shading;
        private readonly IEnergyCalculationService _energy;

        public PlantEnergyService(
            SunPositionService sun,
            IShadingService shading,
            IEnergyCalculationService energy)
        {
            _sun = sun;
            _shading = shading;
            _energy = energy;
        }

        public IReadOnlyList<MonthlyEnergyResult> CalculateMonthlyEnergy(
            Plant plant,
            IEnumerable<SolarIrradiance> data)
        {
            var map = new Dictionary<int, double>();

            foreach (var d in data)
            {
                int month = d.DateTimeLocal.Month;
                if (!map.ContainsKey(month)) map[month] = 0;

                double alt = _sun.GetSolarAltitude(d.DateTimeLocal);

                foreach (var p in plant.Panels)
                {
                    double shade =
                        _shading.GetShadingLoss(p.HeightMeters, alt);

                    map[month] +=
                        _energy.CalculateHourlyEnergy(p, d, alt, shade);
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
