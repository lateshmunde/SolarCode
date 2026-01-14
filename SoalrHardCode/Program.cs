using System;
using System.Collections.Generic;
using SolarEnergyPOC.Data;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;
using SolarEnergyPOC.Services;

namespace SolarEnergyPOC
{
    class Program
    {
        static void Main()
        {
            // --------------------
            // Configuration
            // --------------------
            var location = new Location(23.0, 72.0);
            int year = 2023;
            int panelCount = 18519;

            // --------------------
            // Domain setup
            // --------------------
            var panels = CreatePanels(panelCount);
            var plant = new Plant(location, panels);

            // --------------------
            // Infrastructure setup
            // --------------------
            IIrradianceRepository repo = new NasaPowerIrradianceRepository(location, year);

            var service = new PlantEnergyService(
                new SunPositionService(),
                new ShadingService(),
                new EnergyCalculationService());

            // Fetch data once (important: avoid multiple API calls)
            var data = repo.GetHourlyData();

            // --------------------
            // Practical Case
            // --------------------
            PrintInput("Practical Case : With Losses Considered", location, year, plant, panelCount);

            var MonthlyEnergyList = service.CalculateMonthlyEnergy(plant, data);
            PrintMonthly(MonthlyEnergyList);

            double annual = service.CalculateAnnualEnergy(plant, MonthlyEnergyList);
            Console.WriteLine($"Annual Energy   : {annual / 1_000_000:F3} GWh");

            // --------------------
            // Ideal Case
            // --------------------
            Console.WriteLine("\n------------------------------------------------------------\n");

            PrintInput("Ideal Case : With No Losses Considered", location, year, plant, panelCount);

            var MonthlyEnergyListIdeal = service.CalculateMonthlyEnergyIdeal(plant, data);
            PrintMonthly(MonthlyEnergyListIdeal);

            double annualIdeal = service.CalculateAnnualEnergy(plant, MonthlyEnergyListIdeal);
            Console.WriteLine($"Annual Energy   : {annualIdeal / 1_000_000:F3} GWh");
        }

        private static List<SolarPanel> CreatePanels(int count)
        {
            var panels = new List<SolarPanel>(count);

            for (int i = 0; i < count; i++)
                panels.Add(new SolarPanel(25, 180, 2.5, 0.54));

            return panels;
        }

        private static void PrintInput(string title, Location location, int year, Plant plant, int panelCount)
        {
            Console.WriteLine("INPUT PARAMETERS");
            Console.WriteLine("----------------");
            Console.WriteLine(title);
            Console.WriteLine($"Location        : {location.Latitude}, {location.Longitude}");
            Console.WriteLine($"Year            : {year}");
            Console.WriteLine($"Timezone        : IST (UTC+5:30)");
            Console.WriteLine($"Panel Count     : {panelCount}");
            Console.WriteLine($"Panel Rating    : 0.54 kW");
            Console.WriteLine($"Total DC MW     : {plant.TotalDcCapacityKW / 1000:F2}");
            Console.WriteLine($"Data Source     : NASA POWER");
            Console.WriteLine();
        }

        private static void PrintMonthly(IEnumerable<MonthlyEnergyResult> monthly)
        {
            Console.WriteLine("MONTHLY ENERGY REPORT (GWh)");
            Console.WriteLine("--------------------------");

            foreach (var m in monthly)
            {
                double gwh = m.EnergyKWh / 1_000_000;
                Console.WriteLine($"Month {m.Month:00} : {gwh:F3}");
            }

            Console.WriteLine("--------------------------");
        }
    }
}
