using System;
using System.Collections.Generic;
using System.Linq;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Services
{
    public class PlantEnergyService
    {
        private readonly ISunPositionService _sun;
        private readonly IShadingService _shading;
        private readonly IPlaneOfArrayIrradianceService _poa;
        private readonly IEnergyCalculationService _energy;

        public PlantEnergyService(
            ISunPositionService sun,
            IShadingService shading,
            IPlaneOfArrayIrradianceService poa,
            IEnergyCalculationService energy)
        {
            _sun = sun;
            _shading = shading;
            _poa = poa;
            _energy = energy;
        }

        // ------------------------------------
        // HOURLY → PLANT ENERGY (kWh)
        // ------------------------------------
        private double CalculateHourEnergy(
            IrradianceInput irr,
            PanelGeometry geometry)
        {
            var sun = _sun.GetSunPosition(irr.Timestamp);
            if (!sun.IsSunUp)
                return 0;

            var shadingFactor =
                _shading.CalculateShadingFactor(sun, geometry);

            var poa =
                _poa.CalculatePOA(
                    irr, sun, geometry, shadingFactor);

            return _energy.CalculateFromPOA(poa);
        }

        // ------------------------------------
        // MONTHLY AGGREGATION
        // ------------------------------------
        public IReadOnlyList<MonthlyEnergyResult> CalculateMonthlyEnergy(
            IEnumerable<IrradianceInput> data,
            PanelGeometry geometry,
            int panelCount)
        {
            var grouped =
                data.GroupBy(d => d.Timestamp.Month);

            var results = new List<MonthlyEnergyResult>();

            foreach (var monthGroup in grouped)
            {
                double monthlyEnergy = 0;

                foreach (var hour in monthGroup)
                {
                    monthlyEnergy +=
                        CalculateHourEnergy(hour, geometry);
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

            return results
                .OrderBy(r => r.Month)
                .ToList();
        }

        // ------------------------------------
        // YEARLY ENERGY (from monthly)
        // ------------------------------------
        public double CalculateYearlyEnergy(
            IEnumerable<MonthlyEnergyResult> monthly)
        {
            return monthly.Sum(m => m.EnergyKWh);
        }
    }
}
