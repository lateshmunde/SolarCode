using System;
using System.Collections.Generic;
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
            var location = new Location(23.0, 72.0);
            int year = 2023;
            int panelCount = 18519;

            var panels = new List<SolarPanel>();
            for (int i = 0; i < panelCount; i++)
                panels.Add(new SolarPanel(25, 180, 2.5, 0.54));

            var plant = new Plant(location, panels);

            Console.WriteLine("INPUT PARAMETERS");
            Console.WriteLine("----------------");
            Console.WriteLine("Practical Case : With Losses Considered");
            Console.WriteLine($"Location        : {location.Latitude}, {location.Longitude}");
            Console.WriteLine($"Year            : {year}");
            Console.WriteLine($"Timezone        : IST (UTC+5:30)");
            Console.WriteLine($"Panel Count     : {panelCount}");
            Console.WriteLine($"Panel Rating    : 0.54 kW");
            Console.WriteLine($"Total DC MW     : {plant.TotalDcCapacityKW / 1000:F2}");
            Console.WriteLine($"Data Source     : NASA POWER");
            Console.WriteLine();

            var repo = new NasaPowerIrradianceRepository(location, year);

            var service = new PlantEnergyService(
                new SunPositionService(),
                new ShadingService(),
                new EnergyCalculationService());

            var monthly = service.CalculateMonthlyEnergy(plant, repo.GetHourlyData());

            double annual = 0;

            Console.WriteLine("MONTHLY ENERGY REPORT (GWh)");
            Console.WriteLine("--------------------------");

            foreach (var m in monthly)
            {
                double gwh = m.EnergyKWh / 1_000_000;
                annual += gwh;
                Console.WriteLine($"Month {m.Month:00} : {gwh:F3}");
            }

            Console.WriteLine("--------------------------");
            Console.WriteLine($"Annual Energy   : {annual:F3} GWh");



            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine("INPUT PARAMETERS");
            Console.WriteLine("----------------");
            Console.WriteLine("Ideal Case : With No Losses Considered");
            Console.WriteLine($"Location        : {location.Latitude}, {location.Longitude}");
            Console.WriteLine($"Year            : {year}");
            Console.WriteLine($"Timezone        : IST (UTC+5:30)");
            Console.WriteLine($"Panel Count     : {panelCount}");
            Console.WriteLine($"Panel Rating    : 0.54 kW");
            Console.WriteLine($"Total DC MW     : {plant.TotalDcCapacityKW / 1000:F2}");
            Console.WriteLine($"Data Source     : NASA POWER");
            Console.WriteLine();

            

            var monthlyIdeal = service.CalculateMonthlyEnergyIdeal(plant, repo.GetHourlyData());

            double annualIdeal = 0;

            Console.WriteLine("MONTHLY ENERGY REPORT (GWh)");
            Console.WriteLine("--------------------------");

            foreach (var m in monthlyIdeal)
            {
                double gwh = m.EnergyKWh / 1_000_000;
                annualIdeal += gwh;
                Console.WriteLine($"Month {m.Month:00} : {gwh:F3}");
            }

            Console.WriteLine("--------------------------");
            Console.WriteLine($"Annual Energy   : {annualIdeal:F3} GWh");
        }
    }
}
