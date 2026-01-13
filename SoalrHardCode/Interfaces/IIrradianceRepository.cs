using System.Collections.Generic;
using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Interfaces
{
    /// Contract for retrieving irradiance data - hardcoded, API, database
    public interface IIrradianceRepository
    {
        IEnumerable<SolarIrradiance> GetHourlyData();
    }
}
