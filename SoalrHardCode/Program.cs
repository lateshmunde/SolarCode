using System;
using System.IO;
using System.Linq;
using SolarEnergyPOC.Data;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Services;

namespace SolarEnergyPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===============================================");
            Console.WriteLine("   SOLAR ENERGY MODELLING – MONTHLY REPORT");
            Console.WriteLine("   Data Source : NASA POWER");
            Console.WriteLine("===============================================");

            // -------------------------------------------------
            // 1️⃣ INPUT CONFIGURATION
            // -------------------------------------------------

            int year = 2022;
            var location = new Location(23.0, 72.0); // Ahmedabad, Gujarat

            var panelGeometry = new PanelGeometry
            {
                Tilt = 25,
                Azimuth = 180,
                Albedo = 0.30
            };

            int panelCount = 18519; // fixed as requested (~10 MWp)

            double plantCapacityKW = panelCount * 0.54;

            Console.WriteLine("\n--- INPUT CONFIGURATION ---");
            Console.WriteLine($"Year            : {year}");
            Console.WriteLine($"Latitude        : {location.Latitude}");
            Console.WriteLine($"Longitude       : {location.Longitude}");
            Console.WriteLine($"Panel Tilt      : {panelGeometry.Tilt}°");
            Console.WriteLine($"Panel Azimuth   : {panelGeometry.Azimuth}°");
            Console.WriteLine($"Ground Albedo   : {panelGeometry.Albedo}");
            Console.WriteLine($"Panel Count     : {panelCount}");
            Console.WriteLine($"Plant Capacity : {plantCapacityKW / 1000:F2} MWp");

            // -------------------------------------------------
            // 2️⃣ LOAD NASA POWER DATA
            // -------------------------------------------------

            Console.WriteLine("\n--- LOADING NASA POWER DATA ---");

            var irradianceRepo =
                new NasaPowerIrradianceRepository(location, year);

            var solarData =
                irradianceRepo.GetHourlyData().ToList();

            Console.WriteLine($"Total Hours Retrieved : {solarData.Count}");

            if (solarData.Count == 0)
            {
                Console.WriteLine("❌ ERROR: No data received from NASA POWER.");
                return;
            }

            // -------------------------------------------------
            // 3️⃣ ADAPT DATA (UTC → IST handled internally)
            // -------------------------------------------------

            var irradianceInputs =
                solarData
                .Select(SolarIrradianceAdapter.ToInput)
                .ToList();

            // -------------------------------------------------
            // 4️⃣ INITIALIZE SERVICES
            // -------------------------------------------------

            var sunService = new SunPositionService();
            var shadingService = new ShadingService();
            var poaService = new PlaneOfArrayIrradianceService();
            var energyService = new EnergyCalculationService();

            var plantEnergyService =
                new PlantEnergyService(
                    sunService,
                    shadingService,
                    poaService,
                    energyService);

            // -------------------------------------------------
            // 5️⃣ MONTHLY ENERGY AGGREGATION
            // -------------------------------------------------

            Console.WriteLine("\n--- RUNNING MONTHLY ENERGY AGGREGATION ---");

            var monthlyResults =
                plantEnergyService.CalculateMonthlyEnergy(
                    irradianceInputs,
                    panelGeometry,
                    panelCount);

            double yearlyEnergyKWh =
                plantEnergyService.CalculateYearlyEnergy(monthlyResults);

            // -------------------------------------------------
            // 6️⃣ MONTHLY REPORT (GWh)
            // -------------------------------------------------

            Console.WriteLine("\n--- MONTHLY ENERGY REPORT ---");
            Console.WriteLine("Month | Energy (GWh) | kWh/kWp | CUF (%)");
            Console.WriteLine("----------------------------------------");

            foreach (var m in monthlyResults)
            {
                Console.WriteLine(
                    $"{m.Month,5} | " +
                    $"{m.EnergyKWh / 1_000_000,11:F3} | " +
                    $"{m.SpecificYield,7:F0} | " +
                    $"{m.CUF * 100,6:F2}");
            }

            // -------------------------------------------------
            // 7️⃣ YEARLY SUMMARY (GWh)
            // -------------------------------------------------

            double yearlyEnergyGWh = yearlyEnergyKWh / 1_000_000;
            double yearlySpecificYield =
                yearlyEnergyKWh / plantCapacityKW;
            double yearlyCUF =
                yearlyEnergyKWh / (plantCapacityKW * 8760);

            Console.WriteLine("\n--- YEARLY SUMMARY ---");
            Console.WriteLine($"Yearly Energy     : {yearlyEnergyGWh:F3} GWh");
            Console.WriteLine($"Specific Yield    : {yearlySpecificYield:F0} kWh/kWp/year");
            Console.WriteLine($"CUF               : {yearlyCUF * 100:F2} %");

            Console.WriteLine("\n===============================================");
            Console.WriteLine(" Solar Energy Simulation Completed Successfully");
            Console.WriteLine("===============================================");
        }
    }
}
