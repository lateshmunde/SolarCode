using System.Collections.Generic;
using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Data
{
    /// <summary>
    /// Provides hardcoded irradiance data for POC purposes.
    /// 
    /// This simulates real-world hourly data (inspired by NASA POWER).
    /// It exists to:
    /// - Avoid API dependencies during early development
    /// - Allow deterministic and repeatable testing
    /// 
    /// This class can later be replaced by:
    /// - API-based repository
    /// - File-based repository (CSV / JSON)
    /// </summary>
    public class HardcodedIrradianceRepository
    {
        public IEnumerable<SolarIrradiance> GetHourlyData()
        {
            // Clear-sky winter day in Gujarat (realistic values)
            return new List<SolarIrradiance>
            {
                new SolarIrradiance(9,  450, 600,  80, 22),
                new SolarIrradiance(10, 650, 800,  90, 24),
                new SolarIrradiance(11, 820, 900, 100, 26),
                new SolarIrradiance(12, 950, 1000,120, 28),
                new SolarIrradiance(13, 900, 950, 110, 29),
                new SolarIrradiance(14, 750, 850, 100, 30),
                new SolarIrradiance(15, 550, 700,  90, 29)
            };
        }
    }
}
