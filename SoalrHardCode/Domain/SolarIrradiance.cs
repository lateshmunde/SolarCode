using System;

namespace SolarEnergyPOC.Domain
{
    public class SolarIrradiance
    {
        public DateTime DateTimeLocal { get; }
        public double Ghi { get; }
        public double Dni { get; }
        public double Dhi { get; }
        public double AmbientTempC { get; }

 
        public SolarIrradiance( DateTime local, double ghi, double dni, double dhi, double ambientTempC)
        {
            DateTimeLocal = local;
            Ghi = ghi;
            Dni = dni;
            Dhi = dhi;
            AmbientTempC = ambientTempC;
        }
    }
}
