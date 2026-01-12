using System.Collections.Generic;
using SolarEnergyPOC.Domain;

namespace SolarEnergyPOC.Interfaces
{
    /// <summary>
    /// Contract for retrieving irradiance data.
    /// Source can be hardcoded, API, database, or file.
    /// </summary>
    public interface IIrradianceRepository
    {
        IEnumerable<SolarIrradiance> GetHourlyData();
    }
}
