using System;

namespace SolarEnergyPOC.Domain
{
    /// <summary>
    /// Represents irradiance data at a specific timestamp.
    /// Units: W/m²
    /// </summary>
    public class SolarIrradiance
    {
        public DateTime Timestamp { get; }

        public double Ghi { get; }
        public double Dni { get; }
        public double Dhi { get; }

        public double AmbientTempC { get; }

        public SolarIrradiance(
            DateTime timestamp,
            double ghi,
            double dni,
            double dhi,
            double ambientTempC)
        {
            Timestamp = timestamp;
            Ghi = ghi;
            Dni = dni;
            Dhi = dhi;
            AmbientTempC = ambientTempC;
        }
    }
}
