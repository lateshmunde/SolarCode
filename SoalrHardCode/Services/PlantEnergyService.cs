using System.Collections.Generic;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Services.Losses;

namespace SolarEnergyPOC.Services
{
    public class PlantEnergyService
    {
        private readonly SunPositionService sun;
        private readonly PoaTranspositionService poa;
        private readonly LossPipeline pipeline;

        public PlantEnergyService(
            SunPositionService sun,
            PoaTranspositionService poa,
            LossPipeline pipeline)
        {
            this.sun = sun;
            this.poa = poa;
            this.pipeline = pipeline;
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

                double alt = sun.GetSolarAltitude(d.DateTimeLocal);

                foreach (var p in plant.Panels)
                {
                    var ctx = new EnergyContext
                    {
                        Ghi = d.Ghi,
                        Dni = d.Dni,
                        Dhi = d.Dhi,
                        SunAltitudeDeg = alt,
                        CellTemperatureC = d.AmbientTempC,
                        Poa = poa.CalculatePoa(p, d),
                        DcPowerKW = p.RatedPowerKW * (poa.CalculatePoa(p, d) / 1000.0)
                    };

                    map[month] += pipeline.Run(ctx, p);
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
