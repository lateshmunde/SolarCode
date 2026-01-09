using System;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Data;
using SolarEnergyPOC.Services;

namespace SolarEnergyPOC
{
    /// <summary>
    /// Entry point and orchestration layer.
    /// 
    /// This file wires together domain, data, and services.
    /// No business logic should live here.
    /// </summary>
    class Program
    {
        static void Main()
        {
            var panel = new SolarPanel(tiltDeg: 25, azimuthDeg: 180, heightMeters: 2.5, ratedPowerKW: 0.54);

            var irradianceRepo = new HardcodedIrradianceRepository();
            var sunService = new SunPositionService();
            var shadingService = new ShadingService();
            var energyService = new EnergyCalculationService();

            double totalEnergy = 0;

            foreach (var data in irradianceRepo.GetHourlyData())
            {
                double sunAltitude = sunService.GetSolarAltitudeDeg(data.Hour);
                double shadingLoss = shadingService.GetShadingLoss(panel.HeightMeters,sunAltitude);

                double energy = energyService.CalculateHourlyEnergy(panel, data, sunAltitude, shadingLoss);

                totalEnergy += energy;

                Console.WriteLine(
                    $"Hour {data.Hour}: {energy:F2} kWh | Shading: {shadingLoss:P0}"
                );
            }

            Console.WriteLine("----------------------------------");
            Console.WriteLine($"Total Daily Energy: {totalEnergy:F2} kWh");
        }
    }
}
