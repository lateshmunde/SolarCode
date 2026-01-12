using System;
using System.Collections.Generic;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Data;
using SolarEnergyPOC.Interfaces;
using SolarEnergyPOC.Services;

namespace SolarEnergyPOC
{
    class Program
    {
        static void Main()
        {
            var location = new Location(23.0, 72.0);
            int year = 2023;

            int panelCount = 18519;
            var panels = new List<SolarPanel>();

            for (int i = 0; i < panelCount; i++)
            {
                panels.Add(new SolarPanel(
                    tiltDeg: 25,
                    azimuthDeg: 180,
                    heightMeters: 2.5,
                    ratedPowerKW: 0.54
                ));
            }

            var plant = new Plant(location, panels);

            IIrradianceRepository irradianceRepo =
                new NasaPowerIrradianceRepository(location, year);

            var plantEnergyService = new PlantEnergyService(
                new SunPositionService(),
                new ShadingService(),
                new EnergyCalculationService()
            );

            var monthlyResults =
                plantEnergyService.CalculateMonthlyEnergy(
                    plant,
                    irradianceRepo.GetHourlyData(),
                    year
                );

            double yearlyEnergy =
                plantEnergyService.CalculateYearlyEnergy(monthlyResults);

            Console.WriteLine("Monthly Energy Report");
            Console.WriteLine("--------------------------------");

            foreach (var month in monthlyResults)
            {
                Console.WriteLine(
                    $"Month {month.Month:00} : {month.EnergyKWh / 1_000:F2} MWh"
                );
            }

            Console.WriteLine("--------------------------------");
            Console.WriteLine(
                $"Annual Energy : {yearlyEnergy / 1_000_000:F2} GWh"
            );
        }
    }
}
