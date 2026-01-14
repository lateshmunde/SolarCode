using System;
using System.Collections.Generic;
using SolarEnergyPOC.Data;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;
using SolarEnergyPOC.Services;
using SolarEnergyPOC.Services.Losses;

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
            // Data source (UNCHANGED)
            // --------------------
            IIrradianceRepository repo = new NasaPowerIrradianceRepository(location, year);

            var irradianceData = repo.GetHourlyData();

            // --------------------
            // Services
            // --------------------
            var sunService = new SunPositionService();
            var poaService = new PoaTranspositionService();

            // --------------------
            // Loss Pipeline (PVcase-style)
            // --------------------
            var losses = new List<IEnergyLoss>
            {
                new ShadingLoss(),
                //new SoilingLoss(0.05),        // 5% soiling
                new SoilingLoss(0.00),       
                new TemperatureLoss(),
                //new DcWiringLoss(0.02),       // 2% DC wiring
                new DcWiringLoss(0.00),       // 2% DC wiring
                new InverterLoss(0.97)        // 97% inverter efficiency
            };

            var pipeline = new LossPipeline(losses);

            // --------------------
            // Plant Energy Engine
            // --------------------
            var plantEnergyService = new PlantEnergyService(
                sunService,
                poaService,
                pipeline);

            // --------------------
            // Run calculation
            // --------------------
            PrintInput(location, year, plant, panelCount);

            var monthly = plantEnergyService.CalculateMonthlyEnergy(plant, irradianceData);

            PrintMonthly(monthly);

            double annual = 0;
            foreach (var m in monthly)
                annual += m.EnergyKWh;

            Console.WriteLine($"Annual Energy   : {annual / 1_000_000:F3} GWh");
        }

        private static List<SolarPanel> CreatePanels(int count)
        {
            var panels = new List<SolarPanel>(count);
            for (int i = 0; i < count; i++)
                panels.Add(new SolarPanel(25, 180, 2.5, 0.54));
            return panels;
        }

        private static void PrintInput(Location location, int year, Plant plant, int panelCount)
        {
            Console.WriteLine("INPUT PARAMETERS");
            Console.WriteLine("----------------");
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
