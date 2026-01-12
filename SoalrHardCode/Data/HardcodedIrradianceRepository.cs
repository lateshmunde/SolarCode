using System;
using System.Collections.Generic;
using SolarEnergyPOC.Domain;
using SolarEnergyPOC.Interfaces;

namespace SolarEnergyPOC.Data
{
    /// <summary>
    /// Provides deterministic, physically reasonable
    /// hourly irradiance data for a clear-sky day in India.
    /// 
    /// Units:
    /// GHI, DNI, DHI → W/m²
    /// </summary>
    public class HardcodedIrradianceRepository : IIrradianceRepository
    {
        public IEnumerable<SolarIrradiance> GetHourlyData()
        {
            var date = new DateTime(2024, 3, 21); // Equinox (good sanity day)

            return new List<SolarIrradiance>
            {
                new SolarIrradiance(date.AddHours(6),  120, 150, 40, 18),
                new SolarIrradiance(date.AddHours(7),  300, 420, 60, 20),
                new SolarIrradiance(date.AddHours(8),  520, 650, 80, 22),
                new SolarIrradiance(date.AddHours(9),  700, 820, 90, 24),
                new SolarIrradiance(date.AddHours(10), 850, 950, 100,26),
                new SolarIrradiance(date.AddHours(11), 950,1050,110,28),
                new SolarIrradiance(date.AddHours(12), 1000,1100,120,30),
                new SolarIrradiance(date.AddHours(13), 950,1050,110,31),
                new SolarIrradiance(date.AddHours(14), 850, 950, 100,32),
                new SolarIrradiance(date.AddHours(15), 700, 820, 90, 31),
                new SolarIrradiance(date.AddHours(16), 480, 600, 70, 29),
                new SolarIrradiance(date.AddHours(17), 250, 350, 50, 27),
                new SolarIrradiance(date.AddHours(18), 80,  100, 30, 25)
            };
        }
    }
}
