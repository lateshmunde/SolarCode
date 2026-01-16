using System.Collections.Generic;
using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Services
{
    public class PlantEnergyService
    {
        private readonly SunPositionService sunService;
        private readonly PoaTranspositionService poaService;
        private readonly LossPipeline lossPipeline;

        public PlantEnergyService(
            SunPositionService sunService,
            PoaTranspositionService poaService,
            LossPipeline lossPipeline)
        {
            this.sunService = sunService;
            this.poaService = poaService;
            this.lossPipeline = lossPipeline;
        }

        // -------------------------------
        // PUBLIC: Monthly aggregation
        // -------------------------------
        public IReadOnlyList<MonthlyEnergyResult> CalculateMonthlyEnergy(Plant plant, IEnumerable<SolarIrradiance> hourlyData)
        {
            var monthlyMap = new Dictionary<int, double>();

            foreach (var hour in hourlyData)
            {
                int month = hour.DateTimeLocal.Month;
                if (!monthlyMap.ContainsKey(month))
                    monthlyMap[month] = 0;

                foreach (var panel in plant.Panels)
                {
                    monthlyMap[month] +=
                        CalculateHourlyEnergy(panel, hour, plant.Location);
                }
            }

            var result = new List<MonthlyEnergyResult>();
            foreach (var kv in monthlyMap)
                result.Add(new MonthlyEnergyResult(kv.Key, kv.Value));

            result.Sort((a, b) => a.Month.CompareTo(b.Month));
            return result;
        }

        private double CalculateHourlyEnergy(SolarPanel panel, SolarIrradiance irradiance, Location loc)
        {
            // 1. Solar geometry
            double sunAltitude = sunService.GetSolarAltitude(irradiance.DateTimeLocal, loc);

            if (sunAltitude <= 0)
                return 0.0;

            // 2. Plane-of-Array irradiance (W/m²)
            double poa = poaService.CalculatePoa(panel, irradiance);

            if (poa <= 0)
                return 0.0;

            // 3. Initial DC power (before losses)
            // RatedPower * (POA / 1000)
            double dcPower = panel.RatedPowerKW * (poa / 1000.0);

            // 4. Build energy context (hour snapshot)
            var ctx = new EnergyContext
            {
                Irradiance = irradiance,
                SunAltitudeDeg = sunAltitude,
                Poa = poa,
                CellTemperatureC = irradiance.AmbientTempC,
                DcPowerKW = dcPower
            };

            // 5. Run loss pipeline (DC → AC → Energy)
            return lossPipeline.Run(ctx, panel);
        }
    }
}
