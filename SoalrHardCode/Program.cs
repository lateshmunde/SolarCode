using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            // Configuration
            //string cityName = "Khavda"; // Hardcoded
            //var location = new Location(23.8443, 69.7317);

            string cityName = "Chinchwad"; // Hardcoded
            var location = new Location(18.618, 73.801);

            //string cityName = "Pune"; // Hardcoded
            //var location = new Location(18.52, 73.88);


            int year = 2023;
            int panelCount = 18519;

            // Domain setup
            var panels = CreatePanels(panelCount);
            var plant = new Plant(location, panels);

            // Data source
            IIrradianceRepository repo = new NasaPowerIrradianceRepository(location, year);
            var irradianceData = repo.GetHourlyData();

            // Services
            var sunService = new SunPositionService();
            var poaService = new PoaTranspositionService();

            // Site Layout Params - Critical sun Altitude
            DateTime winterDesignTime =
                new DateTime(2025, 12, 21, 9, 0, 0); // local clock time

            double criticalSunAltitudeDeg =
                sunService.GetSolarAltitude(winterDesignTime, location);

            var layoutParameters = new SiteLayoutParameters
            {
                CriticalSunAltitudeDeg = criticalSunAltitudeDeg
            };


            // Loss Pipeline
            var losses = new List<IEnergyLoss>
            {
                new ShadingLoss(layoutParameters),
                new SoilingLoss(0.05),
                new TemperatureLoss(),
                new DcWiringLoss(0.02),
                new InverterLoss(0.97)
            };

            var pipeline = new LossPipeline(losses);

            var plantEnergyService = new PlantEnergyService(
                sunService,
                poaService,
                pipeline);

            // Run
            var monthly = plantEnergyService.CalculateMonthlyEnergy(plant, irradianceData);

            PrintConsoleReport(cityName, location, plant, panelCount, year, monthly);

            ExportToReport(cityName, location, plant, panelCount, year, monthly,
                $"Solar_Report_{cityName}_{year}.txt");

            Console.WriteLine("\nReports generated successfully.");
        }

        private static List<SolarPanel> CreatePanels(int count)
        {
            var list = new List<SolarPanel>(count);
            for (int i = 0; i < count; i++)
                list.Add(new SolarPanel(25, 180, 2.5, 0.54));
            return list;
        }

        // CONSOLE REPORT
        private static void PrintConsoleReport(
            string city,
            Location location,
            Plant plant,
            int panelCount,
            int year,
            IEnumerable<MonthlyEnergyResult> monthly)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("         SOLAR PLANT ENERGY REPORT");
            Console.WriteLine("=================================================");
            Console.WriteLine($"Location        : {city}");
            Console.WriteLine($"Coordinates     : {location.Latitude}, {location.Longitude}");
            Console.WriteLine($"Year            : {year}");
            Console.WriteLine($"Panel Count     : {panelCount}");
            Console.WriteLine($"Panel Rating    : 540 W");
            Console.WriteLine($"Total DC Size    : {plant.TotalDcCapacityKW / 1000:F2} MW");
            Console.WriteLine();
            Console.WriteLine("Month        Energy (GWh)");
            Console.WriteLine("------------------------------");

            double total = 0;

            foreach (var m in monthly)
            {
                string name = new DateTime(year, m.Month, 1).ToString("MMMM");
                double gwh = m.EnergyKWh / 1_000_000;
                total += gwh;

                Console.WriteLine($"{name,-12}{gwh,8:F3}");
            }

            Console.WriteLine("------------------------------");
            Console.WriteLine($"Annual Total : {total:F3} GWh");
            Console.WriteLine("=================================================");
        }
   
        //  TXT REPORT
        private static void ExportToReport(
            string city,
            Location location,
            Plant plant,
            int panelCount,
            int year,
            IEnumerable<MonthlyEnergyResult> monthly,
            string path)
        {
            var sb = new StringBuilder();
            double total = 0;

            sb.AppendLine("============================================================");
            sb.AppendLine("                 SOLAR PLANT ENERGY REPORT");
            sb.AppendLine("============================================================\n");

            sb.AppendLine($"Location        : {city}");
            sb.AppendLine($"Coordinates     : {location.Latitude}, {location.Longitude}");
            sb.AppendLine($"Year            : {year}");
            sb.AppendLine($"Panel Count     : {panelCount}");
            sb.AppendLine($"Panel Rating    : 540 W");
            sb.AppendLine($"Total DC Size    : {plant.TotalDcCapacityKW / 1000:F2} MW");
            sb.AppendLine("Data Source      : NASA POWER\n");

            sb.AppendLine("------------------------------------------------------------");
            sb.AppendLine("MONTHLY ENERGY GENERATION");
            sb.AppendLine("------------------------------------------------------------");
            sb.AppendLine($"{"Month",-15}{"Energy (GWh)",15}");
            sb.AppendLine("------------------------------------------------------------");

            foreach (var m in monthly)
            {
                string name = new DateTime(year, m.Month, 1).ToString("MMMM");
                double gwh = m.EnergyKWh / 1_000_000;
                total += gwh;

                sb.AppendLine($"{name,-15}{gwh,15:F3}");
            }

            sb.AppendLine("------------------------------------------------------------");
            sb.AppendLine($"Annual Total     : {total:F3} GWh");
            sb.AppendLine("------------------------------------------------------------");

            File.WriteAllText(path, sb.ToString());
        }
    }
}
