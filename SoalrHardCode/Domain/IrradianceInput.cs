using System;

namespace SolarEnergyPOC.Domain
{
    public class IrradianceInput
    {
        public DateTime Timestamp { get; }

        public double GHI { get; }
        public double DNI { get; }
        public double DHI { get; }

        public IrradianceInput(DateTime timestamp, double ghi, double dni, double dhi)
        {
            Timestamp = timestamp;
            GHI = ghi;
            DNI = dni;
            DHI = dhi;
        }
    }
}
