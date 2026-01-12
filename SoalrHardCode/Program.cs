using System;
using System.Collections.Generic;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Data;
using SolarEnergyPOC.Interfaces;
using SolarEnergyPOC.Services;
/// <summary>
/// Entry point and orchestration layer.
/// 
/// This file wires together domain, data, and services.
/// No business logic should live here.
/// </summary>
namespace SolarEnergyPOC
{
    class Program
    {
        static void Main()
        {
            // -----------------------------
            // 1. Create Plant Geometry
            // -----------------------------

            var location = new Location(23.0, 72.0); // Gujarat

            // Assume 10 MWp plant using 540 Wp modules
            int panelCount = 18519; // 10,000 kW / 0.54 kW

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

            // -----------------------------
            // 2. Wire Services (Interfaces)
            // -----------------------------

            //IIrradianceRepository irradianceRepo =
            //    new HardcodedIrradianceRepository();

            // Replace hardcoded repository
            IIrradianceRepository irradianceRepo =
                new NasaPowerIrradianceRepository(location, year: 2023);


            ISunPositionService sunService =
                new SunPositionService();

            IShadingService shadingService =
                new ShadingService();

            IEnergyCalculationService energyService =
                new EnergyCalculationService();

            var plantEnergyService =
                new PlantEnergyService(
                    sunService,
                    shadingService,
                    energyService
                );

            // -----------------------------
            // 3. Energy Calculations
            // -----------------------------

            double dailyEnergy =
                plantEnergyService.CalculateDailyEnergy(
                    plant,
                    irradianceRepo.GetHourlyData()
                );

            double yearlyEnergy =
                plantEnergyService.ScaleDailyToYearly(dailyEnergy);

            // -----------------------------
            // 4. Reporting
            // -----------------------------

            Console.WriteLine("Solar Plant Energy Report");
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"DC Capacity      : {plant.TotalDcCapacityKW / 1000:F2} MWp");
            Console.WriteLine($"Daily Energy     : {dailyEnergy / 1000:F2} MWh");
            Console.WriteLine($"Annual Energy    : {yearlyEnergy / 1_000_000:F2} GWh");
        }
    }
}
